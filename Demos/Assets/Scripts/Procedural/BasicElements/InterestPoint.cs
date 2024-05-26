using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/InterestPoint")]

[Serializable]
public class InterestPoint : ScriptableObject
{
    /// <summary>
    /// Type Of Intesrest Point
    /// </summary>
    public enum Type_of_POI
    {
        Resources,
        Enemy,
        Mission,
        Allies,
        Construction
    };
    [Header("General")]
    [Tooltip("Tipo de objeto")]
    [SerializeField]
    public Type_of_POI typeOfPoint;

    [Tooltip("Modelo u objeto de este punto de interes")]
    [SerializeField]
    public GameObject objectInstance;

    [Tooltip("Numero maximo de instancias")]
    [SerializeField]
    public int amount;

    [Tooltip("Numero maximo de intentos de generacion")]
    [SerializeField]
    public int attempts;


    [Header("Generate Settings")]
    [Tooltip("Radio minimo de distancia entre instancias")]
    [SerializeField]
    public float radius;

    [Tooltip("Altura maxima a la que se va a generar")]
    [SerializeField]
    public int maxHeight;

    [Tooltip("Altura minima a la que se va a generar")]
    [SerializeField]
    public int minHeight;

    [Tooltip("Biomas en los que aparecera este objeto")]
    [SerializeField]
    public Biome[] biomes;


    private PoissonDiscSampler poissonDisc;

    //Se deberia generar el objeto dentro del Chunk correspondiente dependiendo de la posicion
    public void Generate(int mapSize, MapInfo map, float sizePerBlock, int chunkSize, Transform grandParent)
    {
        HashSet<Vector2> objectPositions = map.getObjects();
        MapGenerator mapGen = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>();
        poissonDisc = new PoissonDiscSampler(mapSize, radius, amount, attempts, maxHeight, minHeight, map, mapGen, biomes);
        List<Vector2> points = poissonDisc.Samples();
        var parent = new GameObject("InterestPoint_" + this.name);
        parent.transform.parent = grandParent;
        foreach (Vector2 sample in points)
        {
            Vector3 pos = new Vector3(sample.x * sizePerBlock - chunkSize / 2 + 1, 
                map.HeightMap[(int)sample.x, (int)sample.y], 
                -sample.y * sizePerBlock + chunkSize / 2 - 1);
            
            objectPositions.Add(pos);
            
            Instantiate(objectInstance, pos, Quaternion.identity, parent.transform);
        }
        map.SetObjectsMap(objectPositions);
    }
}
