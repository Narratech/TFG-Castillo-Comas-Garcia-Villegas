using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
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


        //var copyBiomes = ordenarDensity();

        float defaultThreshold = 2 / (float)biomes.Length;


        float[] thresholds = new float[biomes.Length];

        thresholds[0] = biomes[0].density * defaultThreshold;

        for (int i = 1; i < biomes.Length; i++)
            thresholds[i] = thresholds[i - 1] + biomes[i].density * defaultThreshold;
        

       
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var ruido = (heat[i, j] + moisture[i, j]) / 2;
               
                float positionValue = ruido * thresholds[thresholds.Length-1];

                int currentBiomeIndex=0;
                for (int x = 1; x < biomes.Length; x++)
                { 
                    if (positionValue  <= thresholds[x])
                    {
                        //currentBiomeIndex = biomes[x].Item1;
                        currentBiomeIndex = x;
                        break;
                    }

                }

                biomeMap[i, j] = currentBiomeIndex;
            }
        }


    }

    Tuple<int,int>[] ordenarDensity()
    {
        //ITEM1 INDEX DEL BIOMES ARRAY Y ITEM2 DENSIDAD DEL BIOMA
        Tuple<int, int>[] biomesDensity = new Tuple<int,int>[biomes.Length];
        for (int i = 0; i < biomes.Length; i++) biomesDensity[i] = new Tuple<int, int>(i, biomes[i].density);

        Array.Sort(biomesDensity, (x, y) => x.Item2.CompareTo(y.Item2));
       
        return biomesDensity;
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
