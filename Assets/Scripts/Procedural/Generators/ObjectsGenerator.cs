using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generar Objectos en el mapa
/// </summary>
public static class ObjectsGenerator {
    public static void GenerateObjects (MapInfo mapInfo, BiomeGenerator biomeGenerator, Dictionary<Vector2, Chunk> chunks, float heightPerBlock)
    {
        var mapSize = mapInfo.Size;
        var chunkSize = mapInfo.ChunkSize;

        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / -2f;

        HashSet<Vector2> objectsGenerated = new HashSet<Vector2>();

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                //Si no hay un gameObject generado anteriormente
                if (!objectsGenerated.Contains(new Vector2(x,y))){
                    //Cell current = cellMap[x, y];

                    //VER DE K BIOMA ES ESA CASILLA

                    var currentBiome = biomeGenerator.GetBiomeAt(x, y);
                    var objectsToGenerate = currentBiome.getFolliage();
                    //Ordeno por orden de densidad para q sea equivalente
                    foreach (var obj in objectsToGenerate.OrderBy(o => o.Density))
                    {
                        float noiseValue = Mathf.PerlinNoise(x * obj.NoiseScale, y * obj.NoiseScale);

                        //Si el objecto se puede generar en la capa 
                        //if (obj.GenerationLayer == "")
                        {
                            //Aplico un valor Random sobre la densidad para que sea mas aleatorio
                            float v = Random.Range(0.0f, obj.Density * obj.densityCurve.Evaluate(mapInfo.HeightMap[x, y]- currentBiome.GetMinimumHeight()/(currentBiome.GetMaximumHeight() -currentBiome.GetMinimumHeight())));
                            if (noiseValue < v)
                            {

                                Vector2 chunkPos = new Vector2((int)(x / chunkSize),(int)(y / chunkSize));
                                GameObject generated = GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectsGenerated.transform);
                                
                                generated.transform.position = new Vector3( x * heightPerBlock - chunkSize / 2 + 1, mapInfo.HeightMap[x,y],  -y * heightPerBlock + chunkSize / 2 - 1);

                                generated.transform.rotation = Quaternion.Euler(Random.Range(0, obj.rotation.x), Random.Range(0, obj.rotation.y), Random.Range(0, obj.rotation.z));
                                generated.transform.localScale = obj.scale * Random.Range(obj.minHeight, obj.maxHeight);

                                objectsGenerated.Add(new Vector2(x, y));


                                break;
                            }
                        }

                    }
                }
            }
        }
    }
}
