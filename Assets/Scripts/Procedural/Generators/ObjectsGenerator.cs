using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Generar Objectos en el mapa
/// </summary>
public static class ObjectsGenerator {
    public static IEnumerator GenerateObjects (MapInfo mapInfo, BiomeGenerator biomeGenerator, Dictionary<Vector2, Chunk> chunks, float sizePerBlock,int mapSize)
    {
        bool done = false;

        // Espera un frame
        yield return null; // Tema de que se creen bien los colliders de la malla del mapa

        var chunkSize = mapInfo.ChunkSize;

        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / -2f;

        Dictionary<Vector2,bool> objectsGenerated = mapInfo.getObjects();

        for (int y = 0; y <  mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                //VER EL BIOMA PARA ACCEDER A LSO OBJECTOS DE ESE BIOMA
                var currentBiome = biomeGenerator.GetBiomeAt(x, y);
                var objectsToGenerate = currentBiome.getFolliage();

                //Ordeno por orden de densidad para q sea equivalente
                foreach (var obj in objectsToGenerate.OrderBy(o => o.density))
                {
                    if (!objectsGenerated.ContainsKey(new Vector2(x, y)) || ( obj.folliage && objectsGenerated.ContainsKey(new Vector2(x, y)) && !objectsGenerated[new Vector2(x, y)]) )
                    {
                        
                        float noiseValue = Mathf.PerlinNoise(x * obj.noiseScale, y * obj.noiseScale);

                        //Aplico un valor Random sobre la densidad para que sea mas aleatorio
                        float v = Random.Range(0.0f, obj.density * obj.densityCurve.Evaluate(mapInfo.HeightMap[x, y] - currentBiome.GetMinimumHeight() / (currentBiome.GetMaximumHeight() - currentBiome.GetMinimumHeight())));

                        if (noiseValue < v)
                        {
                            //Debug.Log("Pos " + x + " " + y + " : " + (objectsGenerated.ContainsKey(new Vector2(x, y)) && objectsGenerated[new Vector2(x, y)]));

                            Vector3 posHeight = new Vector3(x * sizePerBlock - chunkSize / 2 + 1, mapInfo.HeightMap[x, y], -y * sizePerBlock + chunkSize / 2 - 1);

                            Vector2 chunkPos = new Vector2((int)(x / chunkSize), (int)(y / chunkSize));
                            GameObject generated = GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectsGenerated.transform);

                            posHeight = objectFloor(posHeight, obj.subsidence_in_the_ground);

                            generated.transform.position = posHeight;

                            //ROTATION
                            generated.transform.rotation = RandomRotation(obj);

                            //SCALE
                            generated.transform.localScale = RandomScale(obj);

                            //HEIGHT
                            if (obj.useRandomHeight)
                                generated.transform.localScale = new Vector3(generated.transform.localScale.x, Random.Range(obj.minMaxHeight.x, obj.minMaxHeight.y), generated.transform.localScale.z);

                            //ADD TO LIST
                            //Debug.Log("Tam List: " + objectsGenerated.Count);
                            OccupySpace(new Vector2(x, y), obj.unitSpace, obj.folliage, objectsGenerated);
                            objectsGenerated[new Vector2(x, y)] = true;
                            //Debug.Log("Tam List: "+objectsGenerated.Count);
                            break;
                        }
                    }
                }
                
            }
        }

        mapInfo.SetObjectsMap(objectsGenerated);
        done = true;

        yield return new WaitWhile(() => done == false);
    }
    public static void OccupySpace(Vector2 pos,int unitSpace,bool anyMore, Dictionary<Vector2,bool> objectsGenerated){
        if (unitSpace<0||objectsGenerated.ContainsKey(pos)) return;
        objectsGenerated.Add(pos, anyMore);
        OccupySpace(pos + Vector2.up,    unitSpace - 1, anyMore, objectsGenerated);
        OccupySpace(pos + Vector2.down,  unitSpace - 1, anyMore, objectsGenerated);
        OccupySpace(pos + Vector2.left,  unitSpace - 1, anyMore, objectsGenerated);
        OccupySpace(pos + Vector2.right, unitSpace - 1, anyMore, objectsGenerated);
    }

    public static Vector3 objectFloor(Vector3 pos, float subsidence_in_the_ground)
    {

        Ray ray = new Ray(pos, Vector3.down*20);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo,20f))
            return hitInfo.point - new Vector3(0, subsidence_in_the_ground, 0);
        
        else return pos;
    }

    public static Quaternion RandomRotation(Foliage obj)
    {
       return obj.randomRotation ? Quaternion.Euler(Random.Range(obj.rotation.x, obj.maxRotation.x), Random.Range(obj.rotation.y, obj.maxRotation.y), Random.Range(obj.rotation.z, obj.maxRotation.z)):
                        Quaternion.Euler(obj.rotation.x, obj.rotation.y, obj.rotation.z);
    }

    public static Vector3 RandomScale(Foliage obj)
    {
        return obj.randomScale ? new Vector3(Random.Range(obj.scale.x, obj.maxScale.x), Random.Range(obj.scale.y, obj.maxScale.y), Random.Range(obj.scale.z, obj.maxScale.z)) :
                                    obj.scale;
    }

}
