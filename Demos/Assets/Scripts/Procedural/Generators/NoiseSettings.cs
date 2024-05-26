using System;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    /// <summary>
    ///  El factor de escala del ruido generado.Un valor mayor producir� un ruido con detalles m�s finos
    /// </summary>
    [Header("Noise generation")]
    [HideInInspector]
    public float noiseScale;
    /// <summary>
    /// El n�mero de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
    /// A medida que se agregan m�s octavas, el ruido generado se vuelve m�s detallado
    /// </summary>
    [SerializeField]
    [Tooltip("Complejidad y detallado. Cada octava es una capa. El resultado final es la superposicion de todas.")]
    [Range(0, 5)]
    public int octaves;
    /// <summary>
    ///  La persistencia controla la amplitud de cada octava.Un valor m�s bajo reducir� el efecto de las octavas posteriores de las octavas posteriores
    /// </summary>
    [SerializeField]
    [Tooltip("Amplitud de las octavas")]
    [Range(0f, 1f)]
    public float persistance;
    /// <summary>
    ///Un multiplicador que determina qu� tan r�pido aumenta la frecuencia para cada octava sucesiva en una funci�n de ruido de Perlin
    /// </summary>
    [Tooltip("Frecuencia de las octavas")]
    [SerializeField]
    public float lacunarity;

    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public Vector2 offset;
}
