using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Biome")]

public class Biome : ScriptableObject
{
    [SerializeField]
    float noiseScale = 50f;

    [SerializeField]
    int octaves = 2;

    [SerializeField]
    [Range(0f, 1f)]
    float persistance = 0.5f;

    [SerializeField]
    float lacunarity = 12;

    [SerializeField]
    float heightMultiplier = 100f;

    [SerializeField]
    AnimationCurve meshHeightCurve;

    [SerializeField]
    [Range(0f, 1f)]
    float weight = 0.5f;

    [SerializeField]
    Foliage[] foliages = null;

    float[,] noiseMap = null;
}
