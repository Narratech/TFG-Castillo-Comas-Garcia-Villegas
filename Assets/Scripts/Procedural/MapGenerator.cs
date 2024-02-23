using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Generador de mapas Procedurales
/// </summary>
public class MapGenerator : MonoBehaviour{
    /// <summary>
    /// Tipo de Configuracion para la generacion
    /// </summary>
    public enum DrawMode {
        /// <summary>
        /// Generacion de un Mapa de Ruido(Solo visual 2D)
        /// </summary>
        NoiseMap,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 2D)
        /// </summary>
        ColorMap,
        /// <summary>
        /// Generacion de un Mapa de Ruido con  los bordes del terreno suavizados(Solo visual 2D)
        /// </summary>
        FallOff,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 3D)
        /// </summary>
        CubicMap,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos y los Objectos puestos para generar(Solo visual 3D)
        /// </summary>
        MapWithObjects,
        /// <summary>
        /// Config de ColorMap y CubicMap
        /// </summary>
        NoObjectsWithDisplay,
        All,
        Cartoon
    };

    public DrawMode drawMode;

    /// <summary>
    /// GameObject Padre de todo el mapa3D que se va a generar
    /// </summary>
    public GameObject gameObjectMap3D;
    /// <summary>
    /// Tamaño del Mapa
    /// </summary>
    public long mapSize;

    //TAMAÑO DEL CHUNK (En caso de que se cambie, es probable de k no se genere bien la malla del mapa debido al limite de creacion de vertices de unity por malla)
    [HideInInspector]
    public int chunkSize = 50;
    //TAMAÑO DE CADA CELDA (En caso de modificacion posible solapacion de vertices)
    public float sizePerBlock = 1f;


    [Range(1, 10)]
    public int levelOfDetail;

    /// <summary>
    ///  Controlar al altura del terreno del mundo
    /// </summary>
    public float heightMultiplier = 100f;

    public AnimationCurve meshHeightCurve;

    /// <summary>
    ///  El factor de escala del ruido generado.Un valor mayor producirá un ruido con detalles más finos
    /// </summary>
    public float noiseScale;
    /// <summary>
    /// El número de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
    /// A medida que se agregan más octavas, el ruido generado se vuelve más detallado
    /// </summary>
    public int octaves;
    /// <summary>
    ///  La persistencia controla la amplitud de cada octava.Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores
    /// </summary>
    [Range(0f, 1f)]
    public float persistance;
    /// <summary>
    ///Un multiplicador que determina qué tan rápido aumenta la frecuencia para cada octava sucesiva en una función de ruido de Perlin
    /// </summary>
    public float lacunarity;
    /// <summary>
    /// La semilla aleatoria utilizada para generar el ruido
    /// </summary>
    public int seed;
    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    public Vector2 offset;

    /// <summary>
    ///  Layers de terreno que se pueden generar
    /// </summary>
    public TerrainType[] regions;
    /// <summary>
    ///  Objectso que se pueden generar por el mapa
    /// </summary>
    public ObjectInMap[] objects;

    /// <summary>
    ///  Generar el mapa con forma de isla
    /// </summary>
    public bool isIsland = false;
    float[,] fallOffMap = null;
    /// <summary>
    ///  Cuando se realize un cambio des de el editor, auto actualizar el mapa
    /// </summary>
    public bool autoUpdate = false;
    /// <summary>
    /// Cuando se inicilize este componente autoregenerar el terreno
    /// </summary>
    public bool autoRegenerate = false;

    //Boleano el cual limpia el terreno cuando se actualiza el mapa(SOLO SE ACTIVA EN EJECUCION)
    bool clean = false;
    bool endlessActive = false;
    public bool getEndLessActive() {  return endlessActive; }
    GameObject trashMaps;// GUARDARME MAPAS ANTERIORES "BASURA"

    float[,] noiseMap = null;
    //Sistema de chunks para la generacion del mallado del mapa
    public Dictionary<Vector2, Chunk> map3D/*= new Dictionary<Vector2, Chunk>()*/;
   

    private void Awake(){
        clean = true;
        if(autoRegenerate) GenerateMap();
        if (GetComponent<EndlessTerrain>() != null && !GetComponent<EndlessTerrain>().enabled) endlessActive = false; 
        else if (GetComponent<EndlessTerrain>().enabled) endlessActive = true;
        //else map3D = new Dictionary<Vector2, Chunk>();
        mapSize = chunkSize;
    }

    public bool IsEndlessActive() { return endlessActive; }

    private void OnValidate(){
        if (mapSize < 1) mapSize = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        if (octaves > 6) octaves = 5;
        if (sizePerBlock < 1f) sizePerBlock = 1f;
    }

    public void GenerateEndlessMap()
    {
        map3D = new Dictionary<Vector2, Chunk>();
        if (isIsland)
        {
            fallOffMap = new float[mapSize, mapSize];
            fallOffMap = Noise.GenerateFallOffMap(mapSize);
        }

        mapSize = /*(long)int.MaxValue - 7*/1000;

        if (drawMode == DrawMode.Cartoon)
            noiseMap = Noise.GenerateNoiseMap(mapSize + 1, seed, noiseScale, octaves, persistance, lacunarity, offset);
        else
            noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        calculateChunkSize();
    }
    /// <summary>
    /// Generar el Mapa con los parametros establecidos
    /// </summary>
    public void GenerateMap(){
        map3D = new Dictionary<Vector2, Chunk>();
        //if (GetComponent<EndlessTerrain>() != null && !GetComponent<EndlessTerrain>().enabled) endlessActive = false;
        //else if (GetComponent<EndlessTerrain>().enabled) endlessActive = true;

        if (!endlessActive)
        {
            //Borro todo lo anterior creado para crear un nuevo mapa
            CleanMaps();
           
            //generar isla si se ha activado en la configuracion
            if (isIsland)
            {
                fallOffMap = new float[mapSize, mapSize];
                fallOffMap = Noise.GenerateFallOffMap(mapSize);
            }

            if (drawMode == DrawMode.Cartoon)
                noiseMap = Noise.GenerateNoiseMap(mapSize + 1, seed, noiseScale, octaves, persistance, lacunarity, offset);
            else
                noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);


            MapDisplay display = GetComponent<MapDisplay>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(noiseMap));
                    display.ActiveMap(true);
                    break;
                case DrawMode.ColorMap:
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize));
                    display.ActiveMap(true);
                    Debug.Log("Color Map 2D generado");
                    break;
                case DrawMode.FallOff:
                    display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(Noise.GenerateFallOffMap(mapSize)));
                    display.ActiveMap(true);
                    break;
                case DrawMode.CubicMap:
                    generateChunks_Minecraft();
                    display.ActiveMap(false);
                    break;
                case DrawMode.MapWithObjects:
                    generateChunks_Minecraft();
                    //ObjectsGenerator.GenerateObjects(mapSize, chunkSize, sizePerBlock, cellMap, map3D, objects);
                    display.ActiveMap(false);
                    break;
                case DrawMode.NoObjectsWithDisplay:
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize));
                    generateChunks_Minecraft();
                    display.ActiveMap(true);
                    break;
                case DrawMode.All:
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize));
                    generateChunks_Minecraft();
                    //ObjectsGenerator.GenerateObjects(mapSize, chunkSize, sizePerBlock, cellMap, map3D, objects);
                    display.ActiveMap(true);
                    break;
                case DrawMode.Cartoon:
                    generateChunks_LowPoly();
                    display.ActiveMap(false);
                    break;
            }
        }
        
    }
    /// <summary>
    /// Se usa para generar el mapa 2D a color
    /// </summary>
    /// <param name="fallOffMap"></param>
    /// <returns></returns>
    Color[] generateColorMap(){
        
        Color[] colorMap = new Color[mapSize * mapSize];

        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                if (isIsland) noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);// calculo del nuevo noise con respecto al falloff
                float currentHeight = noiseMap[x, y];

                foreach (var currentRegion in regions)
                {
                    //recorremos y miramos que tipo de terreno se ha generado
                    if (currentHeight <= currentRegion.height)
                    {
                        colorMap[y * mapSize + x] = currentRegion.color;//Color del pixel que tendra la textura del displayMap
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

    public Cell[,] generateChunk_Minecraft(Vector2Int chunkCoord){
        
        Cell[,] cellMap = new Cell[chunkSize+2, chunkSize+2];
        
        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < chunkSize + 2; y++){
            for (int x = 0; x < chunkSize + 2; x++){
                bool existCellDown = x + chunkCoord.x * chunkSize -1 >=0 && y + chunkCoord.y * chunkSize - 1 >= 0;
                bool existCellUp = x + chunkCoord.x * chunkSize - 1 < mapSize && y + chunkCoord.y * chunkSize - 1 < mapSize;
                if (existCellDown && existCellUp)
                {
                    Vector2Int posNoise = new Vector2Int(x + chunkCoord.x * chunkSize - 1, y + chunkCoord.y * chunkSize - 1);
                    if (isIsland) noiseMap[posNoise.x, posNoise.y] = Mathf.Clamp01(noiseMap[posNoise.x, posNoise.y] - fallOffMap[posNoise.x, posNoise.y]);// calculo del nuevo noise con respecto al falloff
                   
                    float currentHeight = noiseMap[posNoise.x, posNoise.y];

                    foreach (var currentRegion in regions)
                    {
                        //recorremos y miramos que tipo de terreno se ha generado
                        if (currentHeight <= currentRegion.height)
                        {
                            currentHeight = (float)Math.Round(currentHeight, 2);
                            //Nos guardamos el estado de la celda que se ha generado
                            cellMap[x, y] = new Cell();
                            cellMap[x, y].type = currentRegion;
                            cellMap[x, y].noise = currentHeight;
                            cellMap[x, y].Height = (float)Math.Round(meshHeightCurve.Evaluate(currentHeight) * heightMultiplier, 1) * 10 * sizePerBlock;

                            break;
                        }
                    }
                }
                else cellMap[x, y] = null;
            }
        }
        return cellMap;
    }
    public Cell[,] generateChunk_LowPoly(Vector2Int chunkCoord)
    {

        Cell[,] cellMap = new Cell[chunkSize + 1, chunkSize + 1];

        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < chunkSize + 1; y++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                Vector2Int posNoise = new Vector2Int(x + chunkCoord.x * chunkSize, y + chunkCoord.y * chunkSize );
                if (isIsland)
                    noiseMap[posNoise.x, posNoise.y] = Mathf.Clamp01(noiseMap[posNoise.x, posNoise.y] - fallOffMap[posNoise.x, posNoise.y]);// calculo del nuevo noise con respecto al falloff
                float currentHeight = noiseMap[posNoise.x, posNoise.y];

                foreach (var currentRegion in regions)
                {
                    //recorremos y miramos que tipo de terreno se ha generado
                    if (currentHeight <= currentRegion.height)
                    {

                        //Nos guardamos el estado de la celda que se ha generado
                        cellMap[x, y] = new Cell();
                        cellMap[x, y].type = currentRegion;
                        cellMap[x, y].noise = currentHeight;
                        cellMap[x, y].Height = meshHeightCurve.Evaluate(currentHeight) * heightMultiplier;

                        break;
                    }
                }
            }
        }
        return cellMap;
    }


    /// <summary>
    /// Calcula las medidas del chunk segun las del mapa. Como maximo cada chunk sera de 50
    /// </summary>
    void calculateChunkSize(){
        chunkSize = 60;
        int divisor = 2;
        while (divisor < mapSize)
        {
            if(mapSize % divisor == 0)
            {
                chunkSize = (int)mapSize / divisor;
                if (chunkSize <= 50) break;
            }

            divisor += 2;
        }
    }

    void generateChunks_LowPoly(){

        calculateChunkSize();

        int numChunks = (int)mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++){
            for (int x = 0; x < numChunks; x++){
                Vector2Int chunkPos = new Vector2Int(x, y);
                map3D[chunkPos] = new Chunk(this,chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, true, levelOfDetail);
            }
        }
    }
    void generateChunks_Minecraft(){

        calculateChunkSize();
        Debug.Log("tamaño de chunk: " + chunkSize);

        int numChunks = (int)mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++)
        {
            for (int x = 0; x < numChunks; x++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                map3D[chunkPos] = new Chunk(this, chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, false, levelOfDetail);
            }

        }
    }

    /// <summary>
    ///  Si yo creo un mapa y lo actualizo, la basura se va quedando ahi pero desactivada(SOLO FUERA DE EJECUCION),
    ///  en ejecucion se elimina el mapa anterior
    /// </summary>
    void CleanMaps()
    {
        if (clean)
        {
            foreach (var chunk in map3D)
                chunk.Value.delete();
            map3D.Clear();
            if (gameObjectMap3D.transform.childCount > 0)
            {
                foreach (Transform childTransform in gameObjectMap3D.transform)
                {
                    GameObject.Destroy(childTransform.gameObject);
                }
            }
        }
        else if (map3D.Count > 0)
        {

            if (trashMaps == null)
            { //POR SI QUIERES ELIMIANR TODA LA BASURA DE GOLPE Q SEA COMODO
                trashMaps = new GameObject("TrashMaps"); trashMaps.transform.SetParent(transform);
                trashMaps.SetActive(false);
            }

            int numChilds = trashMaps.transform.childCount;
            GameObject mapDeprecated = new GameObject("MapDeprecated " + numChilds);
            mapDeprecated.transform.SetParent(trashMaps.transform);
            foreach (var chunk in map3D)
                chunk.Value.setParent(mapDeprecated.transform);
            map3D.Clear();
            //Por si queda algo en el hijo
            if (gameObjectMap3D.transform.childCount > 0)
            {
                foreach (Transform childTransform in gameObjectMap3D.transform)
                {
                    GameObject.Destroy(childTransform.gameObject);
                }
            }
        }
    }
    public float[,] getNoise()
    {
        return noiseMap;
    }
}
