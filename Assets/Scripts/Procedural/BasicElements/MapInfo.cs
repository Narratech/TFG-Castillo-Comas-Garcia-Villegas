using System.Collections.Generic;
using UnityEngine;

public class MapInfo
{
    float[,] noiseMap;
    public float[,] NoiseMap => noiseMap;

    float[,] heightMap;
    public float[,] HeightMap => heightMap;

    Dictionary<Biome, float>[,] biomeInfluences;

    public Dictionary<Biome, float>[,] BiomeInfluences => biomeInfluences;

    List<Vector2> objectsInMap;

    int size;
    int chunkSize;
    float sizePerBlock;
    bool cartoon;

    public int Size => size;
    public float SizePerBlock => sizePerBlock;
    public int ChunkSize => chunkSize;
    public bool Cartoon => cartoon;

    public MapInfo(int mapSize,bool cartoon,float sizePerBlock)
    {
        size = mapSize;
        this.cartoon = cartoon;
        this.sizePerBlock = sizePerBlock;
        noiseMap = null;
        heightMap = null;
        biomeInfluences = null;
        objectsInMap = new List<Vector2>();
    }

    public void SetInfluenceMap(ref Dictionary<Biome, float>[,] influence) { biomeInfluences = influence; }
    public void SetNoiseMap(float[,] noise) { noiseMap = noise; }
    public void SetHeightMap(float[,] height) { heightMap = height; }
    public void SetObjectsMap(List<Vector2> objectsInMap){ this.objectsInMap = objectsInMap; }
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

    public List<Vector2> getObjects()
    {
        return objectsInMap;
    }
}
