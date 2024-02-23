using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Casilla del Mapa que guarda toda la informacion generada por el ruido de perlin, posteriormente evaluada con las Layers del terreno y 
/// los objectos que pueden generarse en ella
/// </summary>
public class Cell{
    /// <summary>
    /// Bioma al que pertenece
    /// </summary>
    public Dictionary<Biome, float> biomeInfluence;
    /// <summary>
    /// Ruido generado
    /// </summary>
    public float noise;
    /// <summary>
    /// Altura de la Casilla
    /// </summary>
    public float Height;
    /// <summary>
    /// Objecto q se ha podido generar encima de la casilla
    /// </summary>
    public GameObject objectGenerated = null;

    public Color GetColor()
    {
        float lerpValue = 0f;
        bool shouldEnd = false;

        Color color1 = Color.black;
        Color color2 = Color.black;
        foreach (var a in biomeInfluence)
        {
            if (!shouldEnd)
            {
                color1 = a.Key.color;
                lerpValue = a.Value;
            }
            else
            {
                color2 = a.Key.color;
                break;
            }
            shouldEnd = true;
        }
        return Color.Lerp(color2, color1, lerpValue);
    }
}
