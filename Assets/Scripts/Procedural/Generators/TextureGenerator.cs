using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class TextureGenerator {
   
    public static Texture2D TextureFromColorMap(Color[] colorMap, int size, float[,] noiseMap=null,bool showMaxMin = false)
    {
        
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        if (noiseMap != null)
        {
            float minH = 0.12f; 
            float maxH = 0.95f;
            if (showMaxMin)
            {
                ShowMinimas(noiseMap, colorMap, size, minH);
                ShowMaximas(noiseMap, colorMap, size, maxH);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        return texture;
    }
    public static Texture2D TextureFromNoiseMap(float[,] noiseMap) {
        
        int size = noiseMap.GetLength(0);

        Color[] colorMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                colorMap[y * size + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }        
        return TextureFromColorMap(colorMap,size);
    }

    public static void ShowMaximas(float[,] noiseMap, Color[] colorMap, int size,float height)
    {
        var maximas = Noise.FindLocalMaxima(noiseMap);
        maximas = maximas.Where(pos => noiseMap[pos.x, pos.y] >= height).OrderBy(pos => noiseMap[pos.x,pos.y]).ToList();
        foreach (var m in maximas)
        {
            colorMap[m.y * size + m.x] = Color.magenta;
        }
    }

    public static void ShowMinimas(float[,] noiseMap, Color[] colorMap, int size,float height)
    {
        var minima = Noise.FindLocalMinima(noiseMap);
        minima = minima.Where(pos => noiseMap[pos.x, pos.y] <= height).OrderBy(pos => noiseMap[pos.x, pos.y]).ToList();
        foreach (var m in minima)
        {
            colorMap[m.y * size + m.x] = Color.yellow;
        }
    }
}
