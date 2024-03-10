using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{
    float[,] noiseMap;
    public float[,] NoiseMap => noiseMap;

    float[,] heightMap;
    public float[,] HeightMap => heightMap;

    Dictionary<Biome, float>[,] biomeInfluences;

    HashSet<Vector2> objectsInMap;

    int size;
    int chunkSize;
    public int Size => size;
    public int ChunkSize => chunkSize;

    public MapInfo(int mapSize)
    {
        size = mapSize;
        noiseMap = null;
        heightMap = null;
        biomeInfluences = null;
        objectsInMap = new HashSet<Vector2>();
    }

    public void SetInfluenceMap(Dictionary<Biome, float>[,] influence) { biomeInfluences = influence; }
    public void SetNoiseMap(float[,] noise) { noiseMap = noise; }
    public void SetHeightMap(float[,] height) { heightMap = height; }
    public void SetObjectsMap(HashSet<Vector2> objectsInMap){ this.objectsInMap = objectsInMap; }
    public void setChunkSize(int chunkSize) { this.chunkSize = chunkSize; }

    public Color GetColorAt(int x, int y)
    {
        float infl1 = 0f;
        float infl2 = 0f;
        Color color1 = Color.black;
        Color color2 = Color.black;

        bool secondPassed = false;

        var influences = biomeInfluences[x, y];

        foreach (var a in influences)
        {
            if (a.Value > infl1)
            {
                color1 = a.Key.color;
                infl1 = a.Value;

                if (infl2 > 0.01f) continue;
            }
            else if (!secondPassed)
            {
                color2 = a.Key.color;
                infl2 = a.Value;
                secondPassed = true;
                continue;
            }

            if (a.Value > infl2)
            {
                color2 = a.Key.color;
                infl2 = a.Value;
            }
        }
        return Color.Lerp(color2, color1, infl1);
    }
}
