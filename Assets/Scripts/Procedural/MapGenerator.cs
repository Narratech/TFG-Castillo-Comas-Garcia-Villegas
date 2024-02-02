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
        NoObjects,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos y los Objectos puestos para generar(Solo visual 3D)
        /// </summary>
        Objects,
        /// <summary>
        /// Config de ColorMap y NoObjects
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
    /// Tama�o del Mapa
    /// </summary>
    public int mapSize;

    //TAMA�O DEL CHUNK (En caso de que se cambie, es probable de k no se genere bien la malla del mapa debido al limite de creacion de vertices de unity por malla)
    public const int chunkSize = 50;
    //TAMA�O DE CADA CELDA (En caso de modificacion posible solapacion de vertices)
    const float sizePerBlock = 0.5f;


    [Range(1, 10)]
    public int levelOfDetail;

    /// <summary>
    ///  Controlar al altura del terreno del mundo
    /// </summary>
    public float heightMultiplayer = 100f;

    public AnimationCurve meshHeightCurve;

    /// <summary>
    ///  El factor de escala del ruido generado.Un valor mayor producir� un ruido con detalles m�s finos
    /// </summary>
    public float noiseScale;
    /// <summary>
    /// El n�mero de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
    /// A medida que se agregan m�s octavas, el ruido generado se vuelve m�s detallado
    /// </summary>
    public int octaves;
    /// <summary>
    ///  La persistencia controla la amplitud de cada octava.Un valor m�s bajo reducir� el efecto de las octavas posteriores de las octavas posteriores
    /// </summary>
    [Range(0f, 1f)]
    public float persistance;
    /// <summary>
    ///Un multiplicador que determina qu� tan r�pido aumenta la frecuencia para cada octava sucesiva en una funci�n de ruido de Perlin
    /// </summary>
    public float lacunarity;
    /// <summary>
    /// La semilla aleatoria utilizada para generar el ruido
    /// </summary>
    public int seed;
    /// <summary>
    ///  La posici�n inicial del ruido generado
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
    public bool useFallOff = false;
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

    GameObject trashMaps;// GUARDARME MAPAS ANTERIORES "BASURA"

    //Sistema de chunks para la generacion del mallado del mapa
    Dictionary<Vector2, Chunk> map3D= new Dictionary<Vector2, Chunk>();

    public Dictionary<Vector2, Chunk> Map3D
    {
        get { return map3D; }
        set { map3D = value; }
    }

    private void Awake(){
        clean = true;
        if(autoRegenerate) GenerateMap();
    }

    private void OnValidate(){
        if (mapSize < 1) mapSize = 1;
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        if (octaves > 6) octaves = 5;
    }

    /// <summary>
    /// Generar el Mapa con los parametros establecidos
    /// </summary>
    public void GenerateMap(){
        //generar el falloff si se ha activado en la configuracion
        if (useFallOff) {
            fallOffMap = new float[mapSize, mapSize];
            fallOffMap = Noise.GenerateFalloffMap(mapSize); 
        }

        cleanMaps();

        MapDisplay display = GetComponent<MapDisplay>();        
        switch (drawMode){
            case DrawMode.NoiseMap:
                float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset, new Vector2Int(0,0));
                display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(noiseMap));
                display.ActiveMap(true);
                break;
            case DrawMode.ColorMap:
                display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize, mapSize));
                display.ActiveMap(true);
                Debug.Log("Color Map 2D generado");
                break;
            case DrawMode.FallOff:
                display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(Noise.GenerateFalloffMap(mapSize)));
                display.ActiveMap(true);
                break;
            case DrawMode.NoObjects:
                generateChunks_Minecraft();
                display.ActiveMap(false);
                break;
            case DrawMode.Objects:
                generateChunks_Minecraft();
                //ObjectsGenerator.GenerateObjects(mapSize, chunkSize, sizePerBlock, cellMap, map3D, objects);
                display.ActiveMap(false);
                break;
            case DrawMode.NoObjectsWithDisplay:
                display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize, mapSize));
                generateChunks_Minecraft();
                display.ActiveMap(true);
                break;
            case DrawMode.All:
                display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize, mapSize));
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
    /// <summary>
    /// Se usa para generar el mapa 2D a color
    /// </summary>
    /// <param name="fallOffMap"></param>
    /// <returns></returns>
    Color[] generateColorMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset, new Vector2Int(0, 0));
        Color[] colorMap = new Color[mapSize * mapSize];

        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                if (useFallOff) noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);// calculo del nuevo noise con respecto al falloff
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

    public Cell[,] generateChunk(Vector2Int chunkCoord){
        //Generar el mapa de ruido
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset, chunkCoord);

        Cell[,] cellMap = new Cell[chunkSize, chunkSize];
        Color[] colorMap = new Color[chunkSize * chunkSize];

        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < chunkSize; y++){
            for (int x = 0; x < chunkSize; x++){

                if (useFallOff) noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOffMap[x, y]);// calculo del nuevo noise con respecto al falloff
                float currentHeight = noiseMap[x, y];

                foreach (var currentRegion in regions){
                    //recorremos y miramos que tipo de terreno se ha generado
                    if (currentHeight <= currentRegion.height){
                        colorMap[y * chunkSize + x] = currentRegion.color;//Color del pixel que tendra la textura del displayMap
                        //Nos guardamos el estado de la celda que se ha generado
                        cellMap[x, y] = new Cell();
                        cellMap[x, y].type = currentRegion;
                        cellMap[x, y].noise = currentHeight;
                        cellMap[x, y].Height = meshHeightCurve.Evaluate(currentHeight) * heightMultiplayer;

                        break;
                    }
                }
            }
        }
        return cellMap;
    }

    void generateChunks_LowPoly(){
        int numChunks = mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++){
            for (int x = 0; x < numChunks; x++){
                Vector2Int chunkPos = new Vector2Int(x, y);
                map3D[chunkPos] = new Chunk(this,chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, true, levelOfDetail);
            }
        }
    }
    void generateChunks_Minecraft()
    {
        int numChunks = mapSize / chunkSize;
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
    void cleanMaps()
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
}
