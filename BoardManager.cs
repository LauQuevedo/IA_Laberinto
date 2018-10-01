using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class BoardManager : MonoBehaviour {
    [Serializable]
    public class Count{
        public int minimo;
        public int maximo;

        //constructor asignable
        public Count(int min, int max){ 
            minimo = min;
            maximo = max;
        }
    }

    //inicialización de variables para dimensión de gameboard
    public int columnas = 15;
    public int filas = 15;
    public int numTerrenos = 0;
    public string[] tamColumnas; //usada por verificarTamTablero()
    List<int> listaTerrenosId = new List<int>();
    //VARIABLES to hold los prefabs que irán en el tablero
    public GameObject exit;
    //escoger terrenos
    public GameObject[] terrenos;
    public GameObject[] player;
    public GameObject paredExterior;
    //delimitar el board
    private Transform boardHolder;

////////////
    //especificar rango para paredes
    public Count wallCount = new Count(5, 9);
    //track de todas las posibles posiciones, y si un objeto está en esa posición o no
    private List<Vector3> gridPositions = new List<Vector3>();

    // -----------------------------------  LECTURA DE TXT   ----------------------------------- 

    //explorer para seleccionar el txt desde el botón de una escena
    public void OpenExplorer(){
        string path = EditorUtility.OpenFilePanel("Overwrite with txt", "", "txt");
        if (path != null){
            //Debug.Log(path);
            comprobarTamTablero(path);
        }
    }

    void comprobarTamTablero(string path){
        StreamReader reader = new StreamReader(path);
        string text = " ";
        bool coincide = true;
        Debug.Log("Ruta: " + path);
        //leer filas
        string[] filas = File.ReadAllLines(path);
        // leer primera fila
        text = reader.ReadLine();

        //leer filas de todo el archivo
        while (text != null)
        {
            //dividir según delimitador
            tamColumnas = text.Split(',');
            //coincidencia
            if (tamColumnas.Length != filas.Length)
            {
                coincide = false;
                break;
            }
            //leer siguiente línea
            text = reader.ReadLine();
        }

        if (coincide)
        { //si coinciden filasXcolumnas
            Debug.Log("Si es simétrico: " + filas.Length + "x" + tamColumnas.Length);
            LecturaTxt(path);
            tamanoTablero(filas.Length);
        }
        else
        {
            Debug.Log("No coinciden dimensiones de terreno, intentelo de nuevo");
        }
    }

    //retornar valor de fila y columna
    int tamanoTablero(int tamano){
        //asignación a variables globales
        columnas = tamano;
        filas = tamano;
        return tamano;
    }

    //Método obtiene id y número de terrenos 
    void LecturaTxt(string path){
        //leer archivo
        StreamReader reader = new StreamReader(path);
        string text = " ";
        //Debug.Log(reader.ReadToEnd());
        List<int> listaTerrenosCoord = new List<int>();
        //otras variables
        string[] celdas;

        // leer primera linea
        text = reader.ReadLine(); //primera fila
        //leer todo el archivo
        while (text != null)
        {
            //dividir según delimitador
            celdas = text.Split(',');
            //leer cada valor ya dividido por delimitador coma
            for (int i = 0; i < celdas.Length; i++)
            {
                int celdaCasteo = Int32.Parse(celdas[i]);
                //1. almacenar celda en lista total de los terrenos
                listaTerrenosCoord.Add(celdaCasteo);
                //2. almacenar id de terrenos (si no hay coincidencia previa)
                compararTerrenoId(celdaCasteo);
            }
            //leer siguiente línea
            text = reader.ReadLine();
        }
        //terminó de leer el archivo
        reader.Close();
        //retornar # de terrenos 
        numTerrenosMetodo();

        //convertir lista a arreglo
        // tamColumnas = listaColumnas.ToArray();
        // comprobarTamanoTablero(tamColumnas, filasTemp);
    }

    void compararTerrenoId(int id)
    {
        //insertar si no existe
        if (!listaTerrenosId.Contains(id))
            listaTerrenosId.Add(id);
    }

    int numTerrenosMetodo()
    {
        Debug.Log("Número de terrenos: " + listaTerrenosId.Count);
        return listaTerrenosId.Count;
    }


    // -----------------------------------  INSTANCIA DE TABLERO   ----------------------------------- 

    void InicializarLista(){
        gridPositions.Clear();
        //llenar lista con posibles posiciones para el tablero
        for (int x = 0; x < columnas; x++){
            for (int y = 0; y < filas; y++){
                //posibles posiciones para poner cosas
                gridPositions.Add(new Vector3(x, y, 0f)); //añadir valores de x,y a la lista
            }
         }
    }

    //definición de paredes que rodean el board
    void BoardSetup(){
        boardHolder = new GameObject("Board").transform;
        //valores negativos y suma para pintar borde   
        for (int x = -1; x < columnas + 1; x++){
            for (int y = -1; y < filas + 1; y++ ){
                //instanciar terrenos como piso
                GameObject toInstantiate = terreno[Random.Range(0, terreno.Length)];

                //checar e instanciar paredes bordeadoras
                if (x == -1 || x == columnas || y == -1 || y == filas ){
                    //instanciar randomly paredes
                    toInstantiate = paredes;
                    //toInstantiate = paredes[Random.Range(0, paredes.Length)];
                }
                //quaternion signfica sin rotación
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }


    //posicionar objetos en el tablero
    void LayoutTerrenos(GameObject tipoTerreno){

        //Instantiate(tipoTerreno)

    }

	// Use this for initialization
    public void SetupScene(){
        BoardSetup();
        InicializarLista();
        //LayoutTerrenos(); //falta implementarlo

        //posicionar el final
        Instantiate(exit, new Vector3(columnas - 1, filas - 1, 0f), Quaternion.identity);
    }
}