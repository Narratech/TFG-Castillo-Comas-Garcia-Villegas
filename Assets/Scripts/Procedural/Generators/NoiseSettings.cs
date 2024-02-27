using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    /// <summary>
    ///  El factor de escala del ruido generado.Un valor mayor producirá un ruido con detalles más finos
    /// </summary>
    [Header("Noise generation")]
    [SerializeField]
    public float noiseScale;
    /// <summary>
    /// El número de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
    /// A medida que se agregan más octavas, el ruido generado se vuelve más detallado
    /// </summary>
    [SerializeField]
    [Range(0, 5)]
    public int octaves;
    /// <summary>
    ///  La persistencia controla la amplitud de cada octava.Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores
    /// </summary>
    [SerializeField]
    [Range(0f, 1f)]
    public float persistance;
    /// <summary>
    ///Un multiplicador que determina qué tan rápido aumenta la frecuencia para cada octava sucesiva en una función de ruido de Perlin
    /// </summary>
    [SerializeField]
    public float lacunarity;

    /// <summary>
    ///  Desplazamiento del ruido generado
    /// </summary>
    [HideInInspector]
    [SerializeField]
    public Vector2 offset;
}
