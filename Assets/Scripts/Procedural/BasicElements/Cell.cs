using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Casilla del Mapa que guarda toda la informacion generada por el ruido de perlin, posteriormente evaluada con las Layers del terreno y 
/// los objectos que pueden generarse en ella
/// </summary>
public class Cell{
    /// <summary>
    /// Tipo de terreno
    /// </summary>
    public TerrainType type;
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
    public GameObject objectGenerated=null;
}
