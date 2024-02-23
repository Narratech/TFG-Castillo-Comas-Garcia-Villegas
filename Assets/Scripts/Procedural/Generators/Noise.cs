using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generar mapas de Ruido de Perlin
/// </summary>
public static  class Noise {

    /// <summary>
    /// Genera un mapa Procedural en base a los siguienets parametros
    /// </summary>
    /// <param name="size">La altura y Anchura del mapa de ruido a generar</param>
    /// <param name="seed">La semilla aleatoria utilizada para generar el ruido</param>
    /// <param name="noiseScale">El factor de escala del ruido generado.Un valor mayor producirá un ruido con detalles más finos</param>
    /// <param name="octaves"> El número de octavas utilizadas en el algoritmo de ruido. Cada octava es una capa de ruido que se suma al resultado final.A medida que se agregan más octavas, el ruido generado se vuelve más detallado</param>
    /// <param name="persistance">La persistencia controla la amplitud de cada octava. Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores</param>
    /// <param name="lacunarity">El lacunaridad controla la frecuencia de cada octava. Un valor más alto aumentará la frecuencia</param>
    /// <param name="offset">La posición inicial del ruido generado</param>
    /// <param name="chunkCoords">HAY Q PONER PARA K SIRVE</param>
    /// <returns></returns>
    
    public static float[,] GenerateNoiseMap(long size, int seed, float noiseScale, int octaves, float persistance, 
        float lacunarity, Vector2 offset){
        
        // Evitar el error al dividir entre 0
        if (noiseScale <= 0) noiseScale = 0.0001f;
        float[,] noiseMap = new float[size, size];
        // Crear una instancia de Random con la semilla proporcionada
        System.Random r = new System.Random(seed);
        
        // Generar vectores de desplazamiento para cada octava
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = r.Next(-10000, 10000) + offset.x;
            float offsetY = r.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        // Inicializar variables para determinar los valores máximos y mínimos de altura del ruido
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfSize = size / 2f;
        
        // Generar el ruido por octavas
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                // Calcular la altura del ruido para cada octava
                for (int i = 0; i < octaves; i++)
                {
                    float smpleX = (x - halfSize) / noiseScale * frequency + octaveOffsets[i].x;
                    float smpleY = (y - halfSize) / noiseScale * frequency + octaveOffsets[i].y;
                    // Obtener el valor de ruido Perlin y ajustarlo al rango [-1, 1]
                    float perlinValue = Mathf.PerlinNoise(smpleX, smpleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance; // Va aumentando
                    frequency *= lacunarity; // Va disminuyendo

                }
                // Actualizar los valores máximos y mínimos de altura del ruido
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                // Guardar la altura del ruido normalizada en el mapa de ruido
                noiseMap[x, y] = noiseHeight;
            }
        }
        // Normalizar los valores de altura del ruido entre 0 y 1
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }


    //public static List<Vector2Int> FindLocalMaxima(float[,] noiseMap){
    //    List<Vector2Int> maximas = new List<Vector2Int>();

    //    for (int x = 0; x < noiseMap.GetLength(0); x++){
    //        for(int y = 0; y < noiseMap.GetLength(1); y++){
    //            var noiseValue =  noiseMap[x, y];
    //            if(CheckNeighbours(x,y,noiseMap,(neighbourNoise) => neighbourNoise < noiseValue)) { 
    //                maximas.Add(new Vector2Int(x,y));
    //            }
    //        }
    //    }
    //    return maximas;
    //}

    //private static bool CheckNeighbours(int x, int y, float[,] noiseMap, Func<object, bool> failCondition){
    //    foreach (var dir in directions){
    //        var newPost = new Vector2Int(x + dir.x,y + dir.y);
    //        if ( newPost.x <0 || newPost.x >= noiseMap.GetLength(0) || newPost.y < 0 || newPost.y >= noiseMap.GetLength(1))
    //            continue;
    //        if (failCondition(noiseMap[x+dir.x,y+dir.y]))return false;

    //        return true;
    //    }
    //    return true;
    //}




    /// <summary>
    /// Generar un mapa de falloff para suavizar los bordes del terreno
    /// </summary>
    public static float[,] GenerateFallOffMap(long size){
        float[,] map = new float[size, size];
        
        for (int i = 0; i < size; i++){
            for (int j = 0; j < size; j++){
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }

    /// <summary>
    /// Ecuacion que ayuda a calcular el valor de atenuación
    /// </summary>
    static float Evaluate(float value){
        float a = 3;
        float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
