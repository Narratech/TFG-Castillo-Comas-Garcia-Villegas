using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Procedural/InterestPoint")]

[Serializable]
public class InterestPoint : ScriptableObject
{
    /*COSAS QUE INTERNAMENTE NECESITARE
        - Acceso a las Cells de cada chunk para saber si esa "casilla ya ha sido ocupada por otro objecto"
        - USO DEL PATRON POISSON DISC para poder distribuir los puntos de interes siguiendo una serie de condiciones especificas como la distancia entre puntos
        - 
    */


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
        poissonDisc = new PoissonDiscSampler(mapWidth, mapHeight, radius,amount);
        var parent = new GameObject("Padre");
        foreach (Vector2 sample in poissonDisc.Samples())
        {
            float z = map.HeightMap[(int)sample.x, (int)sample.y];
            Vector3  pos = new Vector3(sample.x * sizePerBlock - chunkSize / 2 + 1, 
                map.HeightMap[(int)sample.x, 
                (int)sample.y], -sample.y * sizePerBlock + chunkSize / 2 - 1);
            Instantiate(objectInstance, pos, Quaternion.identity, parent.transform);
        }
    }
}
