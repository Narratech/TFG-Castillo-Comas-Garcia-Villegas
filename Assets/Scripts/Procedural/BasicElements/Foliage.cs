using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Foliage")]

public class Foliage : ScriptableObject
{
    public GameObject prefab;

    // Curva de densidad basada en la altura del terreno
    public AnimationCurve densityCurve;

    /// <summary>
    /// Densidad del objecto 
    /// </summary>               
    public float Density = 0.1f;
    /// <summary>
    /// El ruido generado
    /// </summary>
    public float NoiseScale = 0.1f;
    /// <summary>
    /// Capa en la que se puede generar el Objecto
    /// </summary>
    public string GenerationLayer;
}
