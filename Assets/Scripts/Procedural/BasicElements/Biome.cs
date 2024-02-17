using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Biome")]

public class Biome : ScriptableObject
{
    [SerializeField]
    [Range(0f, 1f)]
    float weight = 0.5f;

    [SerializeField]
    float heightMultiplier = 100f;

    [SerializeField]
    AnimationCurve meshHeightCurve;

    [SerializeField]
    Foliage[] foliages = null;
}
