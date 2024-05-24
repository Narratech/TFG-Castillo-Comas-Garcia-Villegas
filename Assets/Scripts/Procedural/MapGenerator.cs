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
    [Tooltip("Tipo de renderizado de mapa")]
    public DrawMode drawMode;

    /// <summary>
    /// GameObject Padre de todo el mapa3D que se va a generar
    /// </summary>
    [Tooltip("Objeto padre de todo el mapa 3D que se va a generar")]
    public GameObject gameObjectMap3D;

    private GameObject trashMaps;

    /// <summary>
    /// Tama�o del Mapa
    /// </summary>
    [Tooltip("Dimensiones del mapa. Es cuadrado, este valor es el lado")]
    public int mapSize;

    [HideInInspector]
    public int chunkSize = 50;
    [Tooltip("Tamaño de los bloques del mapa cúbico")]
    public float sizePerBlock = 1f;

    /// <summary>
    /// La semilla aleatoria utilizada para generar el ruido
    /// </summary>
    [Tooltip("Inicializador de los números aleatorios de Unity, utilizado para generar el ruido aleatoriamente")]
    public int seed;

    /// <summary>
    /// El tamaño del ruido general en todo el mapa
    /// </summary>
    [Tooltip("Escala del mapa de ruido en el que se basa el mapa 3D")]
    public int noiseSize;

    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    [Tooltip("Desplazamiento del ruido generado")]
    public Vector2 offset;

    [SerializeField]
    public BiomeGenerator biomeGenerator;
    [Tooltip("Lista de tipos de puntos de interes a colocar en el mapa")]
    public InterestPoint[] interestPoints;

    /// <summary>
    ///  Generar el mapa con forma de isla
    /// </summary>
    [Tooltip("Si es True el terreno se genera como isla")]
    public bool isIsland = false;
    float[,] fallOffMap = null;
    /// <summary>
    ///  Cuando se realize un cambio des de el editor, auto actualizar el mapa
    /// </summary>
    [Tooltip("Atualización automatica de la escena si se produce algun cambio en el Map Generator. " +
        "Tenga en cuenta que la velocidad de carga depende del número de elementos que hay en la escena. " +
        "No recomendable para escenas con mucha vegetación, puntos de interés y/o mapas mayores de 500")]
    public bool autoUpdate = false;
    /// <summary>
    /// Generate Objects
    /// </summary>
    [Tooltip("Si es True, se generan los objetos de tipo foliage")]
    public bool generateObjects = false;
    /// <summary>
    /// Generar Puntos de Interes
    /// </summary>
    [Tooltip("Si es True, se generan los puntos de interes")]
    public bool generateInterestPoints = false;
    [Tooltip("Material que se le aplicara al mapa")]
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


    /// <summary>
    /// Posicionar al jugador segun el bioma
    /// </summary>

    [Tooltip("Indica si se quiere utilizar o no toda esta funcionalidad")]
    public bool teleportPlayerToBiome = false;
    [Tooltip("Componente transform del jugador al que se desea teletransportar")]
    public Transform playerTransform = null;
    [Tooltip("Bioma en el que apareceria el jugador al generar el terreno")]
    public Biome playerStartingBiome = null;

    // public float 
    [Tooltip("Influencia minima que debe haber del bioma seleccionado en un punto para que sea valido")]
    public float minimumInfluence = .8f;

    public enum SeedType { globalSeed, randomSeed, customizableSeed }

    [Tooltip("GlobalSeed : Se marca true cuando se quiere generar una semilla aleatoria para posicionar al jugador \n" +
        "RandomSeed : Generar una semilla aleatoria cada vez que se genera el terreno \n" +
        "CustomizableSeed : Permite elegir una semilla especifica ")]
    public SeedType seedType;

    [Tooltip("Semilla que define la posicion inicial del jugador decidida aleatoriamente")]
    public int playerStartPositionSeed;



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
        SetupSeeds();

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
                biomeGenerator.GenerateNoises(mapSize + 1, noiseSize, seed, offset);

                map = new MapInfo(mapSize + 1, true, sizePerBlock);
                biomeGenerator.GenerateBiomeMap(seed, mapSize + 1, offset);
            }
            else
            {
                biomeGenerator.GenerateNoises(mapSize, noiseSize, seed, offset);

                map = new MapInfo(mapSize, false, sizePerBlock);
                biomeGenerator.GenerateBiomeMap(seed, mapSize, offset);
            }

            MapDisplay display = GetComponent<MapDisplay>();

            if (display == null)
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

        // Posicionar al jugador solo si el usuario lo ha especificado
        if (teleportPlayerToBiome)
            StartCoroutine(TeleportPlayerToStartingBiome());
    }

    void SetupSeeds()
    {
        // Establece una semilla aleatoria
        SetRandomSeed();

        // Si se quiere usar la semilla global para la posicion aleatoria del jugador
        if (seedType == SeedType.globalSeed)
            playerStartPositionSeed = seed;

        // Elegir una semilla aleatoria para la posicion inicial del jugador
        else if (seedType == SeedType.randomSeed)
            playerStartPositionSeed = UnityEngine.Random.Range(0, 9999);

        // Inicializar la semilla global para generar el terreno
        UnityEngine.Random.InitState(seed);
    }

    // Establecer una semilla aleatoria basada en el tiempo actual
    void SetRandomSeed()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        UnityEngine.Random.InitState(seed);
    }

    // Se encarga de encontrar una posicion aleatoria dentro de un bioma en especifico para el jugador y teletransportarlo ahi
    IEnumerator TeleportPlayerToStartingBiome()
    {
        yield return new WaitForSeconds(0);

        // Inicializar la semilla global para generar el terreno
        UnityEngine.Random.InitState(playerStartPositionSeed);

        Vector2 playerCoordinatesInBiome = GetPlayerCoordinatesInBiome();

        // Obtener la posicion que tendria el jugador, pegado a la superficie
        Vector3 playerPosition = GetPlayerPositionFromCoordinates(playerCoordinatesInBiome);

        playerTransform.position = playerPosition;

        // Volver a la semilla global para generar el terreno
        UnityEngine.Random.InitState(seed);
    }

    Vector2 GetPlayerCoordinatesInBiome()
    {
        // Posicionar al jugador en el bioma indicado
        const int maxTries = 100;
        for (int i = 0; i < maxTries; i++)
        {
            float x = UnityEngine.Random.Range(0, mapSize);
            float z = -UnityEngine.Random.Range(0, mapSize);

            Dictionary<Biome, float> influencesDictionary = GetBiomeInfluencesAt(x, z);

            // Si tiene la influencia de este tipo de bioma lo suficientemente alta
            // Elegirlo como casilla de partida
            if (influencesDictionary.ContainsKey(playerStartingBiome))
            {
                float influenceOfBiome = influencesDictionary[playerStartingBiome];

                if (influenceOfBiome > minimumInfluence)
                    // Raycast para saber la altura a la que posicionar el jugador
                    return new Vector2(x, z);
            }
        }

        Debug.LogError("Coordenada no encontrada para el jugador");
        return new Vector2(0, 0);
    }

    // Obtener la posicion que tendria el jugador, pegado a la superficie
    Vector3 GetPlayerPositionFromCoordinates(Vector2 coor)
    {
        // Lanza el Raycast desde arriba hacia abajo
        Vector3 startPosition = new Vector3(coor.x, maxHeightPossible + 50, coor.y);
        Ray ray = new Ray(startPosition, Vector3.down);
        RaycastHit hit;

        playerTransform.gameObject.SetActive(false);

        // Comprobar en que posicion exacta colisiona el raycast
        if (Physics.Raycast(ray, out hit, 500))
        {
            playerTransform.gameObject.SetActive(true);

            // Obtén la posición del punto de impacto
            Vector3 hitPosition = hit.point;
            // Añadir un pequeño offset para el jugador
            return hitPosition + new Vector3(0, 3, 0);
        }
        else
        {
            Debug.LogError("No Position Detected for player");

            playerTransform.gameObject.SetActive(true);

            return Vector3.zero;
        }
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
            biomeGenerator.GenerateNoises(mapSize + 1, noiseSize, seed, offset);

            map = new MapInfo(mapSize + 1, true, sizePerBlock);

            biomeGenerator.GenerateBiomeMap(seed, mapSize + 1, offset);

            BuildMap();
        }
        else
        {
            biomeGenerator.GenerateNoises(mapSize, noiseSize, seed, offset);

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
                float currentHeight = GetActualHeight(noise[i, j], influences[i, j]) * 10f;

                if (minecraft)
                    currentHeight = Mathf.Round(currentHeight) * sizePerBlock;
                else
                    currentHeight *= sizePerBlock;

                height[i, j] = currentHeight;

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
        else if (gameObjectMap3D.transform.childCount > 0)
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
            float curveResult = biome.Value;
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
            float curveResult = biome.Value;
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
