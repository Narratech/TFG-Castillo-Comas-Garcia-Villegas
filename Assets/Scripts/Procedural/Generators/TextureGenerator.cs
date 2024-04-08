using UnityEngine;

public static class TextureGenerator {
   
    public static Texture2D TextureFromColorMap(Color[] colorMap, int size)
    {
        
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

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
}
