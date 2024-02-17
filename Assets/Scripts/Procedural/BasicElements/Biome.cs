using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Biome")]

public class Biome : ScriptableObject
{
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

    [SerializeField]
    float heightMultiplier = 100f;

    //Temporal (?)
    public Color color;

    [SerializeField]
    AnimationCurve meshHeightCurve;

    [SerializeField]
    [Range(0f, 1f)]
    float weight = 0.5f;

    [SerializeField]
    Foliage[] foliages = null;

    float[,] noiseMap = null;

    public void GenerateNoiseMap(int size, int seed, Vector2 offset)
    {
        if (lacunarity < 1)
            lacunarity = 1;
        noiseMap = Noise.GenerateNoiseMap(size, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    public float NoiseToHeight(float noise)
    {
        return meshHeightCurve.Evaluate(noise) * heightMultiplier;
    }

    public float GetMaximumHeight()
    {
        return heightMultiplier;
    }

    public float this[int index, int index2] {
        get { return noiseMap[index, index2]; }
    }
}
