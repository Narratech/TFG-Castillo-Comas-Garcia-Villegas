using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Generar Objectos en el mapa
/// </summary>
public static class ObjectsGenerator {
    public static void GenerateObjects (MapInfo mapInfo, BiomeGenerator biomeGenerator, Dictionary<Vector2, Chunk> chunks, float sizePerBlock,int mapSize)
    {
        //var mapSize = mapInfo.Size;
        var chunkSize = mapInfo.ChunkSize;

        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / -2f;

        HashSet<Vector2> objectsGenerated = mapInfo.getObjects();

        for (int y = 0; y <  mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                //Si no hay un gameObject generado anteriormente
                if (!objectsGenerated.Contains(new Vector2(x,y))){

                    //VER EL BIOMA PARA ACCEDER A LSO OBJECTOS DE ESE BIOMA
                    var currentBiome = biomeGenerator.GetBiomeAt(x, y);
                    var objectsToGenerate = currentBiome.getFolliage();

                    //Ordeno por orden de densidad para q sea equivalente
                    foreach (var obj in objectsToGenerate.OrderBy(o => o.density))
                    {
                        float noiseValue = Mathf.PerlinNoise(x * obj.noiseScale, y * obj.noiseScale);

                        //Si el objecto se puede generar en la capa 
                        //if (obj.GenerationLayer == "")
                        {
                            //Aplico un valor Random sobre la densidad para que sea mas aleatorio
                            float v = Random.Range(0.0f, obj.density * obj.densityCurve.Evaluate(mapInfo.HeightMap[x, y]- currentBiome.GetMinimumHeight()/(currentBiome.GetMaximumHeight() -currentBiome.GetMinimumHeight())));
                            if (noiseValue < v)
                            {
                                Vector3 posHeight = objectFloor(new Vector3(x * sizePerBlock - chunkSize / 2 + 1, mapInfo.HeightMap[x, y], -y * sizePerBlock + chunkSize / 2 - 1),obj.prefab.transform.localScale/2);
                                Debug.Log("Coords: "+ x+ " y "+ y);
                                Vector2 chunkPos = new Vector2((int)(x / chunkSize),(int)(y / chunkSize));
                                GameObject generated = GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectsGenerated.transform);

                                generated.transform.position = posHeight;

                                //ROTATION
                                generated.transform.rotation = obj.randomRotation ? 
                                    Quaternion.Euler(Random.Range(obj.rotation.x,obj.maxRotation.x), Random.Range(obj.rotation.y,obj.maxRotation.y), Random.Range(obj.rotation.z,obj.maxRotation.z)):
                                    Quaternion.Euler(obj.rotation.x, obj.rotation.y, obj.rotation.z);

                                //SCALE
                                generated.transform.localScale = obj.randomScale ? 
                                    new Vector3(Random.Range(obj.scale.x,obj.maxScale.x), Random.Range(obj.scale.y, obj.maxScale.y), Random.Range(obj.scale.z, obj.maxScale.z)) : 
                                    obj.scale;

                                //HEIGHT
                                if (obj.useRandomHeight)
                                    generated.transform.localScale = new Vector3(generated.transform.localScale.x,Random.Range(obj.minMaxHeight.x, obj.minMaxHeight.y), generated.transform.localScale.z);

                                //ADD TO LIST
                                //Debug.Log("Tam List: " + objectsGenerated.Count);
                                OccupySpace(new Vector2(x, y), obj.unitSpace, objectsGenerated);
                                //Debug.Log("Tam List: "+objectsGenerated.Count);
                                break;
                            }
                        }

                    }
                }
            }
        }

        mapInfo.SetObjectsMap(objectsGenerated);
    }
    public static void OccupySpace(Vector2 pos,int unitSpace, HashSet<Vector2> objectsGenerated){
        if (unitSpace<0||objectsGenerated.Contains(pos)) return;
        objectsGenerated.Add(pos);
        OccupySpace(pos + Vector2.up,    unitSpace - 1,  objectsGenerated);
        OccupySpace(pos + Vector2.down,  unitSpace - 1,  objectsGenerated);
        OccupySpace(pos + Vector2.left,  unitSpace - 1,  objectsGenerated);
        OccupySpace(pos + Vector2.right, unitSpace - 1,  objectsGenerated);
    }

    public static Vector3  objectFloor(Vector3 pos,Vector3 scale)
    {
        Ray ray = new Ray(pos + new Vector3(0,0.1f,0), Vector3.down);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 20f))
            return hitInfo.point + new Vector3(0,scale.y,0);

        else return pos;
    }
}
