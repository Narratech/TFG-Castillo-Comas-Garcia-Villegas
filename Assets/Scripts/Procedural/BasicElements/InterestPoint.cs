using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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

    /// <summary>
    /// Interpolación entre Puntos de Interés
    /// </summary>
    [SerializeField]
    public bool interpolation;

    private PoissonDiscSampler poissonDisc;

    //Se deberia generar el objeto dentro del Chunk correspondiente dependiendo de la posicion
    public void Generate(int mapWidth, int mapHeight, MapInfo map, float sizePerBlock, int chunkSize)
    {
        Dictionary<Vector2, bool> objectPositions = map.getObjects();
        poissonDisc = new PoissonDiscSampler(mapWidth, mapHeight, radius,amount);
        List<Vector2> points = poissonDisc.Samples();
        var parent = new GameObject("Padre");
        foreach (Vector2 sample in points)
        {
            float z = map.HeightMap[(int)sample.x, (int)sample.y];
            Vector3  pos = new Vector3(sample.x * sizePerBlock - chunkSize / 2 + 1, 
                map.HeightMap[(int)sample.x, 
                (int)sample.y], -sample.y * sizePerBlock + chunkSize / 2 - 1);
            objectPositions.Add(pos, true);
            Instantiate(objectInstance, pos, Quaternion.identity, parent.transform);
        }
        map.SetObjectsMap(objectPositions);
    }
}
