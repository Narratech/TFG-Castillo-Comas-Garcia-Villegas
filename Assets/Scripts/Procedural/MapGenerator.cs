using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generador de mapas Procedurales
/// </summary>
public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// Tipo de Configuracion para la generacion
    /// </summary>
    public enum DrawMode
    {
        /// <summary>
        /// Generacion de un Mapa de Ruido(Solo visual 2D)
        /// </summary>
        NoiseMap,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 2D)
        /// </summary>
        ColorMap,
        /// <summary>
        /// Generacion de un Mapa de Con los Biomas establecidos(Solo visual 2D)
        /// </summary>
        BiomeMap,
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

    public enum ALgorithm
    {
        Linear,
        Exponential,
        Logarithmic,
    }

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

    public int chunkSize = 50;
    //TAMA�O DE CADA CELDA (En caso de modificacion posible solapacion de vertices)
    public float sizePerBlock = 1f;

    [Range(1, 10)]
    public int levelOfDetail;

    /// <summary>
    /// La semilla aleatoria utilizada para generar el ruido
    /// </summary>
    public int seed;

    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    public Vector2 offset;

    /// <summary>
    ///  Todos los biomas del mundo (De momento solo pilla los dos primeros)
    /// </summary>
    [SerializeField]
    Biome[] biomes;

    [SerializeField]
    AnimationCurve noiseTransition;

    [SerializeField]
    AnimationCurve heightTransition;

    /// <summary>
    ///  Layers de terreno que se pueden generar
    /// </summary>
    public TerrainType[] regions;

    /// <summary>
    ///  Objectso que se pueden generar por el mapa
    /// </summary>
    public ObjectInMap[] objects;

    /// <summary>
    /// Generar rios 
    /// </summary>
    public bool createRivers = false;

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

    public bool ShowMaximasAndMinimas = false;

    //Boleano el cual limpia el terreno cuando se actualiza el mapa(SOLO SE ACTIVA EN EJECUCION)
    bool clean = false;
    bool endlessActive = false;
    public bool getEndLessActive() { return endlessActive; }
    GameObject trashMaps;// GUARDARME MAPAS ANTERIORES "BASURA"

    float[,] biomeMap = null;

    MapInfo map;

    public MapInfo Map { get { return map; } }

    //Sistema de chunks para la generacion del mallado del mapa
    public Dictionary<Vector2, Chunk> map3D = new Dictionary<Vector2, Chunk>();

    [HideInInspector]
    public float maxHeightPossible;


    private void Awake()
    {
        clean = true;
        if (GetComponent<EndlessTerrain>() != null && !GetComponent<EndlessTerrain>().enabled)
            endlessActive = false;
        else
            map3D = new Dictionary<Vector2, Chunk>();
        endlessActive = true;
    }

    private void OnValidate()
    {
        if (mapSize < 1) mapSize = 1;
        if (sizePerBlock < 1f) sizePerBlock = 1f;
    }

    /// <summary>
    /// Generar el Mapa con los parametros establecidos
    /// </summary>
    public void GenerateMap()
    {
        //Borro todo lo anterior creado para crear un nuevo mapa
        CleanMaps();
        map3D = new Dictionary<Vector2, Chunk>();
        if (GetComponent<EndlessTerrain>() != null && !GetComponent<EndlessTerrain>().enabled)
        {
            endlessActive = false;
            //generar isla si se ha activado en la configuracion
            if (isIsland)
            {
                fallOffMap = new float[mapSize, mapSize];
                fallOffMap = Noise.GenerateFallOffMap(mapSize);
            }

            GenerateBiomeMap();

            foreach (Biome bio in biomes)
            {
                if (bio.GetMaximumHeight() > maxHeightPossible)
                    maxHeightPossible = bio.GetMaximumHeight();
            }

            if (drawMode == DrawMode.Cartoon)
            {
                foreach (var biome in biomes)
                    biome.GenerateNoiseMap(mapSize + 1, seed, offset);

                map = new MapInfo(mapSize + 1);
            }
            else
            {
                foreach (var biome in biomes)
                    biome.GenerateNoiseMap(mapSize, seed, offset);

                map = new MapInfo(mapSize);
            }

            MapDisplay display = GetComponent<MapDisplay>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    BuildMap(false);
                    display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(biomeMap));
                    display.ActiveMap(true);
                    break;
                case DrawMode.ColorMap:
                    BuildMap(true);
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize, map.NoiseMap, ShowMaximasAndMinimas));
                    display.ActiveMap(true);
                    Debug.Log("Color Map 2D generado");
                    break;
                case DrawMode.BiomeMap:

                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateBiomeColorMap(), mapSize, biomeMap));
                    display.ActiveMap(true);
                    Debug.Log("Color Map 2D generado");
                    break;
                case DrawMode.FallOff:
                    display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(Noise.GenerateFallOffMap(mapSize)));
                    display.ActiveMap(true);
                    break;
                case DrawMode.CubicMap:
                    BuildMap(true);
                    generateChunks_Minecraft();
                    display.ActiveMap(false);
                    break;
                case DrawMode.MapWithObjects:
                    BuildMap(true);
                    generateChunks_Minecraft();
                    //ObjectsGenerator.GenerateObjects(mapSize, chunkSize, sizePerBlock, cellMap, map3D, objects);
                    display.ActiveMap(false);
                    break;
                case DrawMode.NoObjectsWithDisplay:
                    BuildMap(true);
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize));
                    generateChunks_Minecraft();
                    display.ActiveMap(true);
                    break;
                case DrawMode.All:
                    BuildMap(true);
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateColorMap(), mapSize));
                    generateChunks_Minecraft();
                    //ObjectsGenerator.GenerateObjects(mapSize, chunkSize, sizePerBlock, cellMap, map3D, objects);
                    display.ActiveMap(true);
                    break;
                case DrawMode.Cartoon:
                    BuildMap(false);
                    generateChunks_LowPoly();
                    display.ActiveMap(false);
                    break;
            }
        }
        else endlessActive = true;
    }

    public void GenerateEndlessMap()
    {
        map3D = new Dictionary<Vector2, Chunk>();

        GenerateBiomeMap();

        foreach (Biome bio in biomes)
        {
            if (bio.GetMaximumHeight() > maxHeightPossible)
                maxHeightPossible = bio.GetMaximumHeight();
        }

        if (drawMode == DrawMode.Cartoon)
        {
            foreach (var biome in biomes)
                biome.GenerateNoiseMap(mapSize + 1, seed, offset);

            map = new MapInfo(mapSize + 1);

            BuildMap();
        }
        else
        {
            foreach (var biome in biomes)
                biome.GenerateNoiseMap(mapSize, seed, offset);

            map = new MapInfo(mapSize);

            BuildMap(true);
        }

        calculateChunkSize();
    }

    private void GenerateBiomeMap()
    {
        if (drawMode == DrawMode.Cartoon)
            biomeMap = new float[mapSize + 1, mapSize + 1];
        else
            biomeMap = new float[mapSize, mapSize];

        for (int i = 0; i < biomeMap.GetLength(0); i++)
        {
            for (int j = 0; j < biomeMap.GetLength(1); j++)
            {
                biomeMap[i, j] = i < (biomeMap.GetLength(0) / 2) ? 0.5f : 1f;
            }
        }
    }

    /// <summary>
    /// Se usa para generar el mapa 2D a color
    /// </summary>
    /// <param name="fallOffMap"></param>
    /// <returns></returns>
    Color[] generateColorMap()
    {

        Color[] colorMap = new Color[mapSize * mapSize];
        var heightMap = map.NoiseMap;
        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                if (isIsland) heightMap[x, y] = Mathf.Clamp01(heightMap[x, y]);// calculo del nuevo noise con respecto al falloff
                float currentHeight = heightMap[x, y];

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


    /// <summary>
    /// Se usa para generar el mapa 2D a color de biomas
    /// </summary>
    /// <param name="fallOffMap"></param>
    /// <returns></returns>
    Color[] generateBiomeColorMap()
    {

        Color[] colorMap = new Color[mapSize * mapSize];

        //Nos guardamos y vemos toda la informacion del mapa generado
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {

                if (isIsland) biomeMap[x, y] = Mathf.Clamp01(biomeMap[x, y]);// calculo del nuevo noise con respecto al falloff
                float currentHeight = biomeMap[x, y];

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

    void generateChunks_LowPoly()
    {

        calculateChunkSize();

        int numChunks = mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++)
        {
            for (int x = 0; x < numChunks; x++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                Chunk generated = new Chunk(this, chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, true, levelOfDetail);
                map3D[chunkPos] = generated;
            }
        }
    }
    void generateChunks_Minecraft()
    {

        calculateChunkSize();
        //Debug.Log("tama�o de chunk: " + chunkSize);

        int numChunks = mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++)
        {
            for (int x = 0; x < numChunks; x++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                Chunk generated = new Chunk(this, chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, false, levelOfDetail);
                map3D[chunkPos] = generated;
            }

        }
    }

    void BuildMap(bool minecraft = false)
    {
        if (minecraft)
            map = new MapInfo(mapSize);
        else
            map = new MapInfo(mapSize + 1);

        if (isIsland)
        {
            fallOffMap = new float[mapSize, mapSize];
            fallOffMap = Noise.GenerateFallOffMap(mapSize);
        }

        float[,] noise = new float[map.Size, map.Size];
        Dictionary<Biome, float>[,] influences = new Dictionary<Biome, float>[map.Size, map.Size];
        for (int x = 0; x < map.Size; x++)
        {
            for (int y = 0; y < map.Size; y++)
            {
                influences[x, y] = GetBiomeInfluence(x, y);
                noise[x, y] = isIsland ? GetCoordinatesNoise(x, y, influences[x, y]) - fallOffMap[x,y] : GetCoordinatesNoise(x, y, influences[x, y]);
                
            }
        }

        map.SetNoiseMap(noise);

        if (minecraft)
        {
            for (int i = 0; i < map.Size; i++)
            {
                for (int j = 0; j < map.Size; j++)
                {
                    noise[i, j] = (float)Math.Round(noise[i, j], 2);
                }
            }
        }

        if (createRivers)
            noise = GetComponent<RiverGenerator>().GenerateRivers(noise);

        map.SetNoiseMap(noise);

        float[,] height = new float[map.Size, map.Size];
        for (int i = 0; i < map.Size; i++)
        {
            for (int j = 0; j < map.Size; j++)
            {
                height[i, j] = GetActualHeight(noise[i, j], influences[i, j]);
                
                if (minecraft)
                    height[i, j] = (float)(Math.Round(height[i, j], 1) * 10 * sizePerBlock);

            }
        }

        map.SetInfluenceMap(influences);
        map.SetHeightMap(height);
    }

    /// <summary>
    /// Calcula las medidas del chunk segun las del mapa. Como maximo cada chunk sera de 50
    /// </summary>
    void calculateChunkSize()
    {
        chunkSize = 60;
        int divisor = 2;
        while (divisor < mapSize)
        {
            if (mapSize % divisor == 0)
            {
                chunkSize = mapSize / divisor;
                if (chunkSize <= 50) break;
            }

            divisor += 2;
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

    /// <summary>
    /// Transforma el nivel de ruido dado a coordenadas de altura 
    /// teniendo en cuenta los par�metros de los biomas influyentes
    /// </summary>
    /// <param name="noiseValue"></param>
    /// <param name="biomeInfluence"></param>
    /// <returns> La altura correspondiente </returns>
    float GetActualHeight(float noiseValue, Dictionary<Biome, float> biomeInfluence)
    {
        float actualHeight = 0;
        float maxHeightPossible = 0;
        foreach (var biome in biomeInfluence)
        {
            float curveResult = heightTransition.Evaluate(biome.Value/* * biome.Key.GetWeight()*/);
            actualHeight += biome.Key.NoiseToHeight(noiseValue) * curveResult;
            maxHeightPossible += curveResult;
        }
        return actualHeight / maxHeightPossible;
    }

    /// <summary>
    /// (TODO) Se encargar� de devolver el ruido transformado correspondiente a todos los biomas cercanos
    /// Ahora solo devuelve el valor del ruido correspondiente del bioma con mas influencia, sin transicion
    /// </summary>
    /// <param name="posNoise"></param>
    /// <returns></returns>
    float GetCoordinatesNoise(Vector2Int posNoise, Dictionary<Biome, float> biomeInfluence)
    {
        return GetCoordinatesNoise(posNoise.x, posNoise.y, biomeInfluence);
    }

    /// <summary>
    /// (TODO) Se encargar� de devolver el ruido transformado correspondiente a todos los biomas cercanos
    /// Ahora solo devuelve el valor del ruido correspondiente del bioma con mas influencia, sin transicion
    /// </summary>
    /// <param name="posNoise"></param>
    /// <returns></returns>
    float GetCoordinatesNoise(int x, int y, Dictionary<Biome, float> biomeInfluence)
    {
        float currentNoise = 0;
        float maxNoisePossible = 0;
        foreach (var biome in biomeInfluence)
        {
            float curveResult = noiseTransition.Evaluate(biome.Value/* * biome.Key.GetWeight()*/);
            currentNoise += biome.Key[x, y] * curveResult;
            maxNoisePossible += curveResult;
        }
        return currentNoise / maxNoisePossible;
    }

    /// <summary>
    /// (TODO) Devolver� un map con los biomas actuales y su influencia en un punto concreto
    /// De momento devuelve 1 si est� en el bioma y 0 si no, sin transici�n
    /// </summary>
    /// <param name="posNoise"> las coordenadas a procesar</param>
    /// <returns></returns>
    Dictionary<Biome, float> GetBiomeInfluence(Vector2Int posNoise)
    {
        return GetBiomeInfluence(posNoise.x, posNoise.y);
    }

    /// <summary>
    /// (TODO) Devolver� un map con los biomas actuales y su influencia en un punto concreto
    /// De momento devuelve 1 si est� en el bioma y 0 si no, sin transici�n
    /// </summary>
    /// <param name="posNoise"> las coordenadas a procesar</param>
    /// <returns></returns>
    Dictionary<Biome, float> GetBiomeInfluence(int x, int y)
    {
        Dictionary<Biome, float> result = new Dictionary<Biome, float>();

        float realBiome0Infl = x / (float)mapSize;
        result.Add(biomes[0], realBiome0Infl * biomes[0].GetWeight());

        float realBiome1Infl = 1f - (x / (float)mapSize);
        result.Add(biomes[1], realBiome1Infl * biomes[1].GetWeight());

        float maxWeights = (result[biomes[0]] + result[biomes[1]]);

        result[biomes[0]] /= maxWeights;
        result[biomes[1]] /= maxWeights;

        return result;
    }

    //public float[,] getNoise()
    //{
    //    return noiseMap;
    //}
}
