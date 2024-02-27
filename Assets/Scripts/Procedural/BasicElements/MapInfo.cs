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

    int size;

    public int Size => size;

    public MapInfo(int mapSize)
    {
        size = mapSize;
        noiseMap = null;
        heightMap = null;
        biomeInfluences = null;
    }

    public void SetInfluenceMap(Dictionary<Biome, float>[,] influence)
    {
        biomeInfluences = influence;
    }

    public void SetNoiseMap(float[,] noise)
    {
        noiseMap = noise;
    }

    public void SetHeightMap(float[,] height)
    {
        heightMap = height;
    }

    public Color GetColorAt(int x, int y)
    {
        float lerpValue = 0f;
        bool shouldEnd = false;

        Color color1 = Color.black;
        Color color2 = Color.black;
        foreach (var a in biomeInfluences[x, y])
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
