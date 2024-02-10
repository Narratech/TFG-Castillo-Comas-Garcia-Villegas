using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
/// <summary>
/// Capa de terreno que se puede generar
/// </summary>
[System.Serializable]
public struct TerrainType
{
    /// <summary>
    /// Nombre Capa De Terreno
    /// </summary>
    public string Layer;
    /// <summary>
    /// Altura
    /// </summary>
    [Range(0f, 1f)]
    public float height;
    /// <summary>
    /// Color de Capa
    /// </summary>
    public Color color;
}

/// <summary>
/// Objecto que se puede generar en el mapa
/// </summary>
[System.Serializable]
public class ObjectInMap
{
    public GameObject prefab;
    /// <summary>
    /// Densidad del objecto 
    /// </summary>               
    public float Density = 0.1f;
    /// <summary>
    /// El ruido generado
    /// </summary>
    public float NoiseScale = 0.1f;
    /// <summary>
    /// Capa en la que se puede generar el Objecto
    /// </summary>
    public string GenerationLayer;
}

/// <summary>
/// Un Chunk es una porcion del Mapa que contiene el suelo, objectos y bordes de una porcion del mapa generado
/// (ES NECESARIO PUES UNITY LIMITA LA CREACION DE VERTICES PARA LAS MAYAS)
/// </summary>
public class Chunk
{
    /// <summary>
    /// Posicion del chunk en el mapa
    /// </summary>
    public Vector2 posMap;

    GameObject chunk;
    GameObject floor;
    GameObject edges;
    public GameObject objectsGenerated;
    Cell[,] mapCells;
    Bounds bound;
    void generateEdgesGameObject()
    {
        edges = new GameObject("Edges " + posMap);
        edges.transform.SetParent(chunk.transform);

        Material edgesMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        edges.AddComponent<MeshFilter>();
        edges.AddComponent<MeshRenderer>().material = edgesMaterial;
    }

    void createGameObjectChunk(Transform parent, Vector2 posMap)
    {
        //Generamos los GameObjects
        chunk = new GameObject("Chunk " + posMap);
        floor = new GameObject("Suelo " + posMap);
        objectsGenerated = new GameObject("Objectos " + posMap);

        //Establecemos la jerarquia de padres
        setParent(parent);
        floor.transform.SetParent(chunk.transform);
        objectsGenerated.transform.SetParent(chunk.transform);
    }

    public Chunk(MapGenerator mapGenerator, Vector2Int posMap, float sizePerBlock, int chunkSize, Transform parent, bool cartoon, int levelOfDetail)
    {
        this.posMap = posMap;
        bound = new Bounds(posMap.ConvertTo<Vector2>(), Vector2.one * chunkSize);

        createGameObjectChunk(parent, posMap);

        if (Mathf.Abs(posMap.x * chunkSize) <= mapGenerator.mapSize && Mathf.Abs(posMap.y * chunkSize) <= mapGenerator.mapSize)
        {
            mapCells = cartoon ? mapGenerator.generateChunk_LowPoly(posMap) : mapGenerator.generateChunk_Minecraft(posMap);

            //Creamos los respectivos materiales para cada malla
            Material sueloMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            floor.AddComponent<MeshFilter>();
            floor.AddComponent<MeshRenderer>().material = sueloMaterial;


            //Generamos la maya
            if (!cartoon)
            { // si es tipo minecraft
                generateEdgesGameObject();

                GenerateTerrainMesh_Minecraft(mapCells, sizePerBlock);

                edges.AddComponent<MeshCollider>();
                GameObjectUtility.SetStaticEditorFlags(edges, StaticEditorFlags.BatchingStatic);
            }
            else GenerateTerrainMesh_LowPoly(mapCells, levelOfDetail);

            floor.AddComponent<MeshCollider>();
            GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.BatchingStatic);
        }
        else
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        }

        chunk.transform.position = new Vector3(posMap.x * chunkSize, 0, -posMap.y * chunkSize);


        obj = mapGenerator.objects[0].prefab;
        GenerateObjects(mapCells, chunkSize);
    }

    GameObject obj;

    void GenerateObjects(Cell[,] cells, int chunkSize)
    {
        int lenght_0 = cells.GetLength(0);
        int lenght_1 = cells.GetLength(1);

        float distanceBetween = (float)chunkSize / (float)lenght_0;

        Vector3 cornerPosition = new Vector3(chunk.transform.position.x - chunkSize / 2, 0, chunk.transform.position.z + chunkSize / 2);

        for (int i = 0; i < lenght_0; i++)
        {
            for (int j = 0; j < lenght_1; j++)
            {
                //Vector3 objPosition = cornerPosition + new Vector3(i * distanceBetween, 0, j * distanceBetween);

                //Ray ray = new Ray(objPosition, Vector3.up);
                //RaycastHit hitInfo;

                //if (Physics.Raycast(ray, out hitInfo))
                //{
                //    objPosition = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                //}
                //else
                //{
                //    ray = new Ray(objPosition, Vector3.down);

                //    if (Physics.Raycast(ray, out hitInfo))
                //    {
                //        objPosition = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                //    }
                //}

                if (UnityEngine.Random.Range(0, 8) == 0)
                {
                    Vector3 objPosition = cornerPosition + new Vector3(i * distanceBetween, cells[i, j].Height, -j * distanceBetween);

                    GameObject thisObject = Transform.Instantiate(obj, objPosition,
                    Quaternion.identity, chunk.transform);
                }
            }
        }
    }



    /// <summary>
    /// Genera la maya del chunk
    /// </summary>
    public void GenerateTerrainMesh_Minecraft(Cell[,] mapaCells, float sizePerBlock)
    {
        int sizeAntiguo = mapCells.GetLength(0);
        Cell[,] matrizReducida = new Cell[sizeAntiguo - 2, sizeAntiguo - 2];

        // Copiar los elementos relevantes a la nueva matriz
        for (int i = 1; i < sizeAntiguo - 1; i++)
            Array.Copy(mapCells, i * sizeAntiguo + 1, matrizReducida, (i - 1) * (sizeAntiguo - 2), sizeAntiguo - 2);


        MeshGenerator.GenerateTerrainMeshChunk(matrizReducida, floor, sizePerBlock);
        MeshGenerator.DrawEdgesChunk(mapaCells, edges, sizePerBlock);

        mapCells = matrizReducida;

    }

    /// <summary>
    /// Genera la maya del chunk
    /// </summary>
    public void GenerateTerrainMesh_LowPoly(Cell[,] mapaCells, int levelOfDetail)
    {
        MeshGenerator.GenerateTerrainMeshChunk_LowPoly(mapaCells, floor, levelOfDetail);
    }

    public void setParent(Transform parent)
    {
        if (chunk != null)
            chunk.transform.SetParent(parent);
    }

    public void delete()
    {
        GameObject.Destroy(chunk.gameObject);
        GameObject.Destroy(edges.gameObject);
        GameObject.Destroy(objectsGenerated.gameObject);
        GameObject.Destroy(floor.gameObject);
    }

    public void Update(Vector2 viewPosition, float maxViewDst)
    {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bound.SqrDistance(viewPosition));
        setVisible(viewerDstFromNearestEdge <= maxViewDst);
    }

    void setVisible(bool visible)
    {
        chunk.gameObject.SetActive(visible);
    }
}
