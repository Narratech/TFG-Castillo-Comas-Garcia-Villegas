using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BiomeGenerator
{
    float[,] heat = null;
    float[,] moisture = null;

    int[,] biomeMap = null;

    [SerializeField]
    int seed = 0;

    [SerializeField]
    float noiseSize;

    [SerializeField]
    int influenceRange = 10;

    [SerializeField]
    Biome[] biomes = null;

    public void GenerateBiomeMap(int size, Vector2 offset)
    {
        NoiseSettings noiseSettings = new()
        {
            noiseScale = noiseSize,
            octaves = 3,
            lacunarity = 1,
            offset = offset,
            persistance = 1
        };
        heat = Noise.GenerateNoiseMap(size, seed + 1, noiseSettings);
        moisture = Noise.GenerateNoiseMap(size, seed, noiseSettings);

        biomeMap = new int[size, size];

        int valueToDivideHeatMap = biomes.Length / 2 + 1;
        int valueToDivideMoistureMap = (biomes.Length / 2) + biomes.Length % 2;

        float heatThresholdValue;
        float moistureThresholdValue;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                heatThresholdValue = 1 / (float)valueToDivideHeatMap;
                moistureThresholdValue = 1 / (float)valueToDivideMoistureMap;
                int currentBiomeIndex = (int)(heat[i, j] / heatThresholdValue) + (int)(moisture[i, j] / moistureThresholdValue);
                biomeMap[i, j] = currentBiomeIndex;
            }
        }
    }

    public Biome GetBiomeAt(int x, int y)
    {
        return biomes[biomeMap[x, y]];
    }

    internal void GenerateNoises(int mapSize, int seed, Vector2 offset)
    {
        foreach (var biome in biomes)
            biome.GenerateNoiseMap(mapSize, seed, offset);
    }

    internal Dictionary<Biome, float> GetBiomeInfluence(int x, int y)
    {
        Dictionary<Biome, float> result = new Dictionary<Biome, float>();
        result.Add(biomes[biomeMap[x, y]], 1f);
        return result;
    }

    internal float GetMaximumPossibleHeight()
    {
        float height = 0f;
        foreach (Biome bio in biomes)
        {
            if (bio.GetMaximumHeight() > height)
                height = bio.GetMaximumHeight();
        }
        return height;
    }
}
