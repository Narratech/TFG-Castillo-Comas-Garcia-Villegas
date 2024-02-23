using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Biome")]

[Serializable]
public class Biome : ScriptableObject
{
    [Header("Noise generation")]
    [SerializeField]
    float noiseScale = 50f;

    [SerializeField]
    [Range(0, 5)]
    int octaves = 2;

    [SerializeField]
    [Range(0f, 1f)]
    float persistance = 0.5f;

    [SerializeField]
    float lacunarity = 12;

    [Header("Biome generation")]
    [SerializeField]
    [Range(0.001f, 1f)]
    float weight = 0.5f;

    [Header("Terrain transformation")]
    [SerializeField]
    float maxHeight = 100f;
    [SerializeField]
    float minHeight = 0f;

    [SerializeField]
    AnimationCurve meshHeightCurve;

    [Header("Foliage settings")]
    [SerializeField]
    Foliage[] foliages = null;

    [Header("Test values")]
    //Temporal (?)
    public Color color;

    float[,] noiseMap = null;

    public void GenerateNoiseMap(int size, int seed, Vector2 offset)
    {
        if (lacunarity < 1)
            lacunarity = 1;
        noiseMap = Noise.GenerateNoiseMap(size, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    public float NoiseToHeight(float noise)
    {
        return meshHeightCurve.Evaluate(noise) * (maxHeight - minHeight) + minHeight;
    }

    public float GetMaximumHeight()
    {
        return minHeight;
    }

    public float GetWeight()
    {
        return weight;
    }

    public float this[int index, int index2] {
        get { return noiseMap[index, index2]; }
    }
}
