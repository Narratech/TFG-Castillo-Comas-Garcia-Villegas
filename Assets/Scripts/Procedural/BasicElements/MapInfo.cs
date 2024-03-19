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

    HashSet<Vector2> objectsInMap;

    int size;
    int chunkSize;
    public int Size => size;
    public int ChunkSize => chunkSize;

    public MapInfo(int mapSize)
    {
        size = mapSize;
        noiseMap = null;
        heightMap = null;
        biomeInfluences = null;
        objectsInMap = new HashSet<Vector2>();
    }

    public void SetInfluenceMap(Dictionary<Biome, float>[,] influence) { biomeInfluences = influence; }
    public void SetNoiseMap(float[,] noise) { noiseMap = noise; }
    public void SetHeightMap(float[,] height) { heightMap = height; }
    public void SetObjectsMap(HashSet<Vector2> objectsInMap){ this.objectsInMap = objectsInMap; }
    public void setChunkSize(int chunkSize) { this.chunkSize = chunkSize; }

    public Color GetColorAt(int x, int y)
    {

        var influences = biomeInfluences[x, y];

        Color result = Color.black;
        float maxInfl = -1f;

        foreach (var a in influences)
        {
            if (a.Value > maxInfl)
            {
                maxInfl = a.Value;
                result = a.Key.color;
            }
        }
        return result;
    }

    public HashSet<Vector2> getObjects()
    {
        return objectsInMap;
    }
}
