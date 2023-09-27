using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generar Objectos en el mapa
/// </summary>
public static class ObjectsGenerator {
   public static void GenerateObjects
        (int mapSize, int chunkSize,float heightPerBlock, Cell[,] cellMap, Dictionary<Vector2,Chunk> chunks, ObjectInMap[] objectsToGenerate){

        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / -2f;

        for (int y = 0; y < mapSize; y++){
            for (int x = 0; x < mapSize; x++){
                //Si no hay un gameObject generado anteriormente
                if (cellMap[x, y].objectGenerated==null){
                    Cell current = cellMap[x, y];
                    //Ordeno por orden de densidad para q sea equivalente
                    foreach (var obj in objectsToGenerate.OrderBy(o => o.Density)){
                        float noiseValue = Mathf.PerlinNoise(x * obj.NoiseScale, y * obj.NoiseScale);                       
                        //Si el objecto se puede generar en la capa 
                        if (obj.GenerationLayer == current.type.Layer){
                            //Aplico un valor Random sobre la densidad para que sea mas aleatorio
                            float v = Random.Range(0.0f, obj.Density);
                            if (noiseValue < v){

                                Vector2 chunkPos = new Vector2(x / chunkSize, y / chunkSize);
                                GameObject generated= GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectsGenerated.transform);

                                generated.transform.position = new Vector3(topLeftX+x, heightPerBlock * current.noise * 100, topLeftX - y);
                                generated.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                                generated.transform.localScale =Vector3.one * Random.Range(0.8f, 1.2f);

                                current.objectGenerated= generated;
                                break;
                            }
                        }
                           
                    }
                }
            }
        }
    }
}
