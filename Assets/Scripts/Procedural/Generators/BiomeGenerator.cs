using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
public class BiomeGenerator
{

    [SerializeField]
    float noiseSize;

    [SerializeField]
    int influenceRange = 10;

    [SerializeField]
    Biome[] biomes = null;

    int[,] biomeMap = null;

    public void GenerateBiomeMap(int seed, int size, Vector2 offset)
    {
        NoiseSettings noiseSettings = new()
        {
            noiseScale = noiseSize,
            octaves = 3,
            lacunarity = 1,
            offset = offset,
            persistance = 1
        };
        float[,] heat = Noise.GenerateNoiseMap(size, seed + 1, noiseSettings);
        float[,] moisture = Noise.GenerateNoiseMap(size, seed + 2, noiseSettings);

        biomeMap = new int[size, size];




        float threshold = 2 / (float)biomes.Length;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float positionValue = heat[i, j] + moisture[i, j];

                int currentBiomeIndex = (int)(positionValue / threshold);

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

        int numCells = 0;

        float d, yDiff, threshold, radiusSq;
        float diameter = (influenceRange * 2) + 1;
        radiusSq = (diameter * diameter) / 4;

        for (int i = -influenceRange; i <= influenceRange; i++)
        {
            int currentY = i + y;
            if (!(currentY >= 0 && currentY < biomeMap.GetLength(1))) continue;

            yDiff = i;
            threshold = radiusSq - (yDiff * yDiff);

            for (int j = -influenceRange; j <= influenceRange; j++)
            {
                int currentX = j + x;
                if (!(currentX >= 0 && currentX < biomeMap.GetLength(0))) continue;

                d = j;
                if ((j * j) <= threshold)
                {
                    if (!result.ContainsKey(biomes[biomeMap[currentX, currentY]]))
                        result.Add(biomes[biomeMap[currentX, currentY]], 0f);
                    result[biomes[biomeMap[currentX, currentY]]] += 1f;
                    numCells++;
                }

            }

        }

        foreach (var biome in biomes)
        {
            if (result.ContainsKey(biome))
            {
                result[biome] /= (float)numCells;
            }
        }
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
