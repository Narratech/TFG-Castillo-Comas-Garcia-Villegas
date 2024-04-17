using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Biome")]

[Serializable]
public class Biome : ScriptableObject
{
    [SerializeField]
    public NoiseSettings noiseSettings;

    [Header("Biome generation")]
    [SerializeField]
    [Range(0.001f, 1f)]
    float weight = 0.5f;

    public float density = 1;

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


    byte[,] noiseMap = null;

    public void GenerateNoiseMap(int size, int seed, Vector2 offset)
    {
        if (noiseSettings.lacunarity < 1) noiseSettings.lacunarity = 1;
        if (noiseSettings.octaves < 0) noiseSettings.octaves = 0;
        if (noiseSettings.octaves > 6) noiseSettings.octaves = 5;

        noiseSettings.offset = offset;

        noiseMap = new byte[size, size];
        float[,] values = Noise.GenerateNoiseMap(size, seed, noiseSettings);

        for (int i = 0; i < values.GetLength(0); ++i) {

            for (int j = 0; j < values.GetLength(1); ++j)
            {
                noiseMap[i, j] = (byte)Mathf.RoundToInt(values[i, j] * byte.MaxValue);
            }
        }
    }

    public float NoiseToHeight(float noise)
    {
        return meshHeightCurve.Evaluate(noise) * (maxHeight - minHeight) + minHeight;
    }

    public float GetMaximumHeight()
    {
        return maxHeight;
    }

    public float GetMinimumHeight()
    {
        return minHeight;
    }

    public float GetWeight()
    {
        return weight;
    }

    public float this[int index, int index2]
    {
        get { return noiseMap[index, index2] / (float)byte.MaxValue; }
    }

    public Foliage[] getFolliage()
    {
        return foliages;
    }

    public static implicit operator string(Biome a)
    {
        return a.name;
    }
}
