using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BiomeGenerator
{

    [SerializeField]
    float noiseSize;

    [SerializeField]
    AnimationCurve biomeTransition;

    public AnimationCurve BiomeTransitionCurve { get { return biomeTransition; } }

    [SerializeField]
    Biome[] biomes = null;

    byte[,] biomeMap = null;

    Dictionary<Biome, float>[,] influences;

    public Dictionary<Biome, float>[,] BiomeInfluences {  get { return influences; } }

    public void GenerateBiomeMap(int seed, int size, Vector2 offset)
    {
        biomes = biomes.Where(b => b != null).ToArray();
        if (biomes.Length > 255)
        {
            Debug.LogError("Maximum number of biomes allowed is 255, please consider reducing the amount, truncating array");
            Array.Copy(biomes, biomes, 255);
        }
        NoiseSettings noiseSettings = new()
        {
            noiseScale = noiseSize,
            octaves = 3,
            lacunarity = 1,
            offset = offset,
            persistance = 1
        };
        float[,] biomesNoise = Noise.GenerateNoiseMap(size, seed + 1, noiseSettings);

        biomeMap = new byte[size, size];

        influences = new Dictionary<Biome, float>[size, size];

        //var copyBiomes = ordenarDensity();

        float defaultThreshold = 1 / (float)biomes.Length;


        float[] thresholds = new float[biomes.Length];

        thresholds[0] = biomes[0].density * defaultThreshold;

        for (int i = 1; i < biomes.Length; i++)
            thresholds[i] = thresholds[i - 1] + biomes[i].density * defaultThreshold;

       

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var ruido = biomesNoise[i, j];
               
                float noiseValue = ruido * thresholds[thresholds.Length-1];

                byte currentBiomeIndex = 0;
                float biomeInfluence = 0f;

                for (byte x = 0; x < biomes.Length; x++)
                { 
                    if (noiseValue <= thresholds[x])
                    {
                        float lowerValue = 0f;
                        if (x > 0)
                            lowerValue = thresholds[x - 1];
                        else
                        {
                            influences[i, j] = new Dictionary<Biome, float>();
                            influences[i, j].Add(biomes[x], biomeTransition.Evaluate(1f));
                            break;
                        }
                        float upperValue = thresholds[x];

                        //Normalizamos
                        biomeInfluence = (noiseValue - lowerValue) / (upperValue - lowerValue);

                        influences[i, j] = new Dictionary<Biome, float>();
                        influences[i, j].Add(biomes[x], biomeTransition.Evaluate(biomeInfluence));
                        influences[i, j].Add(biomes[x - 1], biomeTransition.Evaluate(1 - biomeInfluence));

                        currentBiomeIndex = biomeInfluence > 0.5f? x : (byte)(x - 1);
                        break;
                    }

                }

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

    internal float GetMaximumPossibleHeight()
    {
        biomes = biomes.Where(b => b != null).ToArray();
        float height = 0f;
        foreach (Biome bio in biomes)
        {
            if (bio.GetMaximumHeight() > height)
                height = bio.GetMaximumHeight();
        }
        return height;
    }

    internal Dictionary<Biome, float>[,] GetInfluences()
    {
        return influences;
    }
}
