using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generar Objectos en el mapa
/// </summary>
public static class ObjectsGenerator {
    public static IEnumerator GenerateObjects (MapInfo mapInfo, BiomeGenerator biomeGenerator, Dictionary<Vector2, Chunk> chunks,int mapSize)
    {
        bool done = false;

        // Espera un frame
        yield return null; // Tema de que se creen bien los colliders de la malla del mapa

        var chunkSize = mapInfo.ChunkSize;
        var sizePerBlock = mapInfo.SizePerBlock;

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
                        float v = UnityEngine.Random.Range(0.0f, obj.density * obj.densityCurve.Evaluate(mapInfo.HeightMap[x, y] - currentBiome.GetMinimumHeight() / (currentBiome.GetMaximumHeight() - currentBiome.GetMinimumHeight())));

                        if (noiseValue < v)
                        {
                            Vector3 posHeight = new Vector3(x * sizePerBlock - chunkSize / 2 + 1, mapInfo.HeightMap[x, y], -y * sizePerBlock + chunkSize / 2 - 1); //calculamos la posicion

                            Vector2 chunkPos = new Vector2((int)(x / chunkSize), (int)(y / chunkSize));

                            //instanciamos el objecto teniendo en cuanta el gamobject de objectos del chunk al que pertenece
                            GameObject generated = GameObject.Instantiate(obj.prefab, chunks[chunkPos].objectsGenerated.transform);

                            //POSICIOANR EL OBJECTO
                            Tuple<Vector3, Quaternion> aux = default(Tuple<Vector3, Quaternion>);

                            if (mapInfo.Cartoon)
                            {
                                aux = objectFloor(posHeight, obj.subsidence_in_the_ground);
                                posHeight = aux.Item1;
                            }

                            //ROTATION
                            generated.transform.rotation = RandomRotation(obj);
                            if (mapInfo.Cartoon && obj.environment_rotation) generated.transform.rotation = Quaternion.Slerp(generated.transform.rotation, aux.Item2, Time.deltaTime * 5f);

                            //SCALE
                            generated.transform.localScale = RandomScale(obj); //calculamos primero la escala pq luego la hemos de tener en cuenta en el posicionamiento

                            //HEIGHT
                            if (obj.useRandomHeight)
                                generated.transform.localScale = new Vector3(generated.transform.localScale.x, UnityEngine.Random.Range(obj.minMaxHeight.x, obj.minMaxHeight.y), generated.transform.localScale.z);
                            
                           
                            
                            generated.transform.position = posHeight;

                            //ADD TO LIST
                            OccupySpace(new Vector2(x, y), obj.unitSpace, obj.folliage, objectsGenerated);
                            objectsGenerated[new Vector2(x, y)] = true;

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

    public static Tuple<Vector3,Quaternion> objectFloor(Vector3 pos, float subsidence_in_the_ground)
    {
        Ray ray = new Ray(pos+new Vector3(0,15f,0), Vector3.down*20);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 20f))
        {
            Vector3 normalTerreno = hitInfo.normal;
            Quaternion rotacionObjeto = Quaternion.FromToRotation(Vector3.up, normalTerreno); //obtengo rotacion con respecto al terreno
           
            return new Tuple<Vector3, Quaternion>(hitInfo.point + new Vector3(0, subsidence_in_the_ground, 0),rotacionObjeto);
        }
        else return new Tuple<Vector3,Quaternion>(pos,default(Quaternion));
    }

    public static Quaternion RandomRotation(Foliage obj)
    {
       return obj.randomRotation ? Quaternion.Euler(UnityEngine.Random.Range(obj.rotation.x, obj.maxRotation.x), UnityEngine.Random.Range(obj.rotation.y, obj.maxRotation.y), UnityEngine.Random.Range(obj.rotation.z, obj.maxRotation.z)):
                        Quaternion.Euler(obj.rotation.x, obj.rotation.y, obj.rotation.z);
    }

    public static Vector3 RandomScale(Foliage obj)
    {
        return obj.randomScale ? new Vector3(UnityEngine.Random.Range(obj.scale.x, obj.maxScale.x), UnityEngine.Random.Range(obj.scale.y, obj.maxScale.y), UnityEngine.Random.Range(obj.scale.z, obj.maxScale.z)) :
                                    obj.scale;
    }

}
