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

    [SerializeField]
    public Type_of_POI typeOfPoint;

    [SerializeField]
    public GameObject objectInstance;

    [SerializeField]
    public int amount;


    [Header("Generate Settings")]
    [SerializeField]
    public float radius;

    [Tooltip("Altura maxima a la que se va a generar")]
    [SerializeField]
    public int maxHeight;

    [Tooltip("Altura minima a la que se va a generar")]
    [SerializeField]
    public int minHeight;

    [Tooltip("Bioma en el que aparecerá ese objeto")]
    [SerializeField]
    public Biome[] biomes;


    private PoissonDiscSampler poissonDisc;

    //Se deberia generar el objeto dentro del Chunk correspondiente dependiendo de la posicion
    public void Generate(int mapSize, MapInfo map, float sizePerBlock, int chunkSize, Transform grandParent)
    {
        HashSet<Vector2> objectPositions = map.getObjects();
        MapGenerator mapGen = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>();
        poissonDisc = new PoissonDiscSampler(mapSize, radius,amount, maxHeight, minHeight, map, mapGen, biomes);
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
