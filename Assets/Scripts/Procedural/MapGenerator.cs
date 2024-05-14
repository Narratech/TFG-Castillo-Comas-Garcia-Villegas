using System;
using System.Collections;
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
        /// Generacion de un Mapa de Ruido con  los bordes del terreno suavizados(Solo visual 2D)
        /// </summary>
        FallOff,
        /// <summary>
        /// Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 3D)
        /// </summary>
        CubicMap,
        Cartoon,
        All
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

    private GameObject trashMaps;

    /// <summary>
    /// Tama�o del Mapa
    /// </summary>
    public int mapSize;

    [HideInInspector]
    public int chunkSize = 50;

    public float sizePerBlock = 1f;

    /// <summary>
    /// La semilla aleatoria utilizada para generar el ruido
    /// </summary>
    public int seed;

    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    public Vector2 offset;

    [SerializeField]
    BiomeGenerator biomeGenerator;

    public InterestPoint[] interestPoints;

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
    /// Generate Objects
    /// </summary>
    public bool generateObjects = false;
    /// <summary>
    /// Generar Puntos de Interes
    /// </summary>
    public bool generateInterestPoints = false;

    public Material material;

    //Boleano el cual limpia el terreno cuando se actualiza el mapa(SOLO SE ACTIVA EN EJECUCION)
    bool clean = false;
    bool endlessActive = false;
    public bool getEndLessActive() { return endlessActive; }

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
       StartCoroutine(CleanMaps());

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

            maxHeightPossible = biomeGenerator.GetMaximumPossibleHeight();

            if (drawMode == DrawMode.Cartoon)
            {
                biomeGenerator.GenerateNoises(mapSize + 1, seed, offset);

                map = new MapInfo(mapSize + 1, true, sizePerBlock);
                biomeGenerator.GenerateBiomeMap(seed, mapSize + 1, offset);
            }
            else
            {
                biomeGenerator.GenerateNoises(mapSize, seed, offset);

                map = new MapInfo(mapSize, false, sizePerBlock);
                biomeGenerator.GenerateBiomeMap(seed, mapSize, offset);
            }

            MapDisplay display = GetComponent<MapDisplay>();

            if (display==null)
                display = AddComponent<MapDisplay>();

            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    BuildMap(false);
                    display.DrawTextureMap(TextureGenerator.TextureFromNoiseMap(map.NoiseMap));
                    display.ActiveMap(true);
                    break;

                case DrawMode.ColorMap:
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateBiomeColorMap(), mapSize));
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
                    GenerateInterestPoints();
                    break;
                case DrawMode.Cartoon:
                    BuildMap(false);
                    generateChunks_LowPoly();
                    display.ActiveMap(false);
                    GenerateInterestPoints();
                    break;
                case DrawMode.All:
                    BuildMap(true);
                    display.DrawTextureMap(TextureGenerator.TextureFromColorMap(generateBiomeColorMap(), mapSize));
                    generateChunks_Minecraft();
                    display.ActiveMap(true);
                    GenerateInterestPoints();
                    break;
            }
        }
        else endlessActive = true;
        if (!endlessActive)
            map.setChunkSize(chunkSize);

        GenerateObjects();

    }

    private T AddComponent<T>()
    {
        throw new NotImplementedException();
    }

    public void GenerateEndlessMap()
    {
        //CleanMaps();
        map3D = new Dictionary<Vector2, Chunk>();

        maxHeightPossible = biomeGenerator.GetMaximumPossibleHeight();


        if (drawMode == DrawMode.Cartoon)
        {
            biomeGenerator.GenerateNoises(mapSize + 1, seed, offset);

            map = new MapInfo(mapSize + 1, true, sizePerBlock);

            biomeGenerator.GenerateBiomeMap(seed, mapSize + 1, offset);

            BuildMap();
        }
        else
        {
            biomeGenerator.GenerateNoises(mapSize, seed, offset);

            map = new MapInfo(mapSize, false, sizePerBlock);

            biomeGenerator.GenerateBiomeMap(seed, mapSize, offset);

            BuildMap(true);
        }

        //calculateChunkSize();

        map.setChunkSize(chunkSize);

        GenerateInterestPoints();
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
                colorMap[y * mapSize + (mapSize - x - 1)] = biomeGenerator.GetBiomeAt(x, y).color;
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
                Chunk generated = new Chunk(this, chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, material);
                map3D[chunkPos] = generated;
            }
        }
    }

    void generateChunks_Minecraft()
    {
        calculateChunkSize();
        //Debug.Log("tamano de chunk: " + chunkSize);

        int numChunks = mapSize / chunkSize;
        if (mapSize % chunkSize != 0) numChunks++;

        for (int y = 0; y < numChunks; y++)
        {
            for (int x = 0; x < numChunks; x++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                Chunk generated = new Chunk(this, chunkPos, sizePerBlock, chunkSize, gameObjectMap3D.transform, null);
                map3D[chunkPos] = generated;
            }

        }
    }

    void BuildMap(bool minecraft = false)
    {
        map = !minecraft ? new MapInfo(mapSize + 1, true, sizePerBlock) : new MapInfo(mapSize, false, sizePerBlock);

        if (isIsland)
        {
            fallOffMap = new float[mapSize, mapSize];
            fallOffMap = Noise.GenerateFallOffMap(mapSize);
        }

        float[,] noise = new float[map.Size, map.Size];
        Dictionary<Biome, float>[,] influences = biomeGenerator.GetInfluences();
        for (int x = 0; x < map.Size; x++)
        {
            for (int y = 0; y < map.Size; y++)
            {
                influences[x, y] = influences[x, y];
                noise[x, y] = isIsland ? GetCoordinatesNoise(x, y, influences[x, y]) - fallOffMap[x, y] : GetCoordinatesNoise(x, y, influences[x, y]);

            }
        }

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

        map.SetInfluenceMap(ref influences);
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

    IEnumerator CleanMaps()
    {
        if (clean)
        {
            if (gameObjectMap3D.transform.childCount > 0)
            {

                #if UNITY_EDITOR
                    UnityEngine.Object.DestroyImmediate(gameObjectMap3D);
                #else
                    UnityEngine.Object.Destroy(gameObjectMap3D);
                #endif

                gameObjectMap3D = new GameObject("Mapa3D");
                gameObjectMap3D.transform.SetParent(transform);
                gameObjectMap3D.transform.SetSiblingIndex(1);
            }
        }
        else if(gameObjectMap3D.transform.childCount > 0)
        {
            if (trashMaps == null)
            { //POR SI QUIERES ELIMIANR TODA LA BASURA DE GOLPE Q SEA COMODO
                trashMaps = GameObject.Find("TrashMaps");
                if (trashMaps == null)
                {
                    trashMaps = new GameObject("TrashMaps");
                    trashMaps.transform.SetParent(transform);
                    trashMaps.SetActive(false);
                }
            }

            //Por si queda algo en el hijo
            
            System.DateTime horaActual = System.DateTime.Now;
            gameObjectMap3D.name = "MapDeprecated " + horaActual.ToString("HH:mm:ss");

            gameObjectMap3D.transform.SetParent(trashMaps.transform);
            var basura = gameObjectMap3D;
            gameObjectMap3D = new GameObject("Mapa3D");
               
            gameObjectMap3D.transform.SetParent(transform);
            gameObjectMap3D.transform.SetSiblingIndex(1);
            
        }
        // Espera un frame
        yield return new WaitWhile(() => gameObjectMap3D.transform.parent != transform);
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
            float curveResult = biomeGenerator.BiomeTransitionCurve.Evaluate(biome.Value/* * biome.Key.GetWeight()*/);
            actualHeight += biome.Key.NoiseToHeight(noiseValue) * curveResult * (drawMode == DrawMode.Cartoon ? 10 : 1);
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
            float curveResult = biomeGenerator.BiomeTransitionCurve.Evaluate(biome.Value/* * biome.Key.GetWeight()*/);
            currentNoise += biome.Key[x, y] * curveResult;
            maxNoisePossible += curveResult;
        }
        return currentNoise / maxNoisePossible;
    }

    public Dictionary<Biome, float> GetBiomeInfluencesAt(Vector3 globalPosition)
    {
        return GetBiomeInfluencesAt(globalPosition.x, globalPosition.z);
    }

    public Dictionary<Biome, float> GetBiomeInfluencesAt(Vector2 globalPosition)
    {
        return GetBiomeInfluencesAt(globalPosition.x, globalPosition.y);
    }

    public Dictionary<Biome, float> GetBiomeInfluencesAt(float x, float y)
    {
        float topLeftX = chunkSize / 2;
        float topLeftZ = chunkSize / 2;

        int indexX = Mathf.Clamp(Mathf.RoundToInt((x + topLeftX) - transform.position.x), 0, map.Size - 1);
        int indexY = Mathf.Clamp(Mathf.RoundToInt((-y + topLeftZ) + transform.position.z), 0, map.Size - 1);

        Debug.Log("x: " + indexX + ", y: " + indexY);

        return map.BiomeInfluences[indexX, indexY];
    }

    void GenerateObjects()
    {
        if (generateObjects)
            StartCoroutine(ObjectsGenerator.GenerateObjects(map, biomeGenerator, map3D,
                drawMode == DrawMode.Cartoon ? mapSize - 1 : mapSize
                ));
    }

    /// <summary>
    /// Generar los puntos de interes si estan activos
    /// </summary>
    void GenerateInterestPoints()
    {
        if (!generateInterestPoints) return;
        Debug.Log("Generando Puntos de Interés:");
        foreach (var points in interestPoints)
            points.Generate((int)(mapSize * sizePerBlock), map, sizePerBlock, chunkSize, gameObjectMap3D.transform);

        Debug.Log("Puntos de Interes Generados");
    }

    public int GetMeshSimplificationValue(int LODlevel)
    {
        const float MAXLOD = 4f;

        float simplificationRate = LODlevel / MAXLOD;

        int[] divisors = GetDivisors(chunkSize);

        int indexToGet = Mathf.RoundToInt(simplificationRate * (divisors.Length - 1));
        return divisors[indexToGet];
    }

    // function to count the divisors 
    static int[] GetDivisors(int n)
    {
        List<int> div = new List<int>();
        for (int i = 1; i <= n / 2; i++)
        {
            if (n % i == 0)
            {
                // If divisors are equal, 
                // count only one 
                div.Add(i);
            }
        }

        return div.ToArray();
    }
    public BiomeGenerator GetBiomeGenerator() { return biomeGenerator; }
}
