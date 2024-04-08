using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Un Chunk es una porcion del Mapa que contiene el suelo, objectos y bordes de una porcion del mapa generado
/// (ES NECESARIO PUES UNITY LIMITA LA CREACION DE VERTICES PARA LAS MAYAS)
/// </summary>
public class Chunk
{
    MapGenerator generator;

    /// <summary>
    /// Posicion del chunk en el mapa
    /// </summary>
    public Vector2 posMap;

    GameObject chunk;
    GameObject floor;
    GameObject edges;
    public GameObject objectsGenerated;
    float sizePerBlock;
    Bounds bound;

    Vector2Int horBounds = Vector2Int.zero;
    Vector2Int verBounds = Vector2Int.zero;

    void generateEdgesGameObject()
    {
        edges = new GameObject("Edges " + posMap);
        edges.transform.SetParent(chunk.transform);

        Material edgesMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        edges.AddComponent<MeshFilter>();
        edges.AddComponent<MeshRenderer>().material = edgesMaterial;
        GameObjectUtility.SetStaticEditorFlags(edges, StaticEditorFlags.BatchingStatic);
    }

    void createGameObjectChunk(Transform parent)
    {
        //Generamos los GameObjects
        chunk = new GameObject("Chunk " + posMap);
        //chunk = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor = new GameObject("Suelo " + posMap);
        objectsGenerated = new GameObject("Objectos " + posMap);

        //Establecemos la jerarquia de padres
        setParent(parent);
        floor.transform.SetParent(chunk.transform);
        objectsGenerated.transform.SetParent(chunk.transform);
    }

    public Chunk(MapGenerator mapGenerator, Vector2Int posMap, float sizePerBlock, int chunkSize, Transform parent,Material mat)
    {

        generator = mapGenerator;

        this.posMap = posMap;
        createGameObjectChunk(parent);
        this.sizePerBlock = sizePerBlock;


        if (mapGenerator.getEndLessActive()) //si esta activado endless terrain se generaran los chunks de esta manera
        {
            Vector2 realPos = posMap * chunkSize;
            bound = new Bounds(realPos, Vector2.one * chunkSize);
        }

        else
            bound = new Bounds(new Vector2(posMap.x,posMap.y), Vector2.one * chunkSize);
        

        //Creamos los respectivos materiales para cada malla
        Material sueloMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floor.AddComponent<MeshFilter>();
        floor.AddComponent<MeshRenderer>().material = mapGenerator.drawMode == MapGenerator.DrawMode.Cartoon ? mat: sueloMaterial;


        //Generamos la malla
        if (!(mapGenerator.drawMode == MapGenerator.DrawMode.Cartoon))
        { // si es tipo minecraft
            horBounds = new Vector2Int(posMap.x * chunkSize, (posMap.x * chunkSize) + chunkSize);
            verBounds = new Vector2Int(posMap.y * chunkSize, (posMap.y * chunkSize) + chunkSize);

            generateEdgesGameObject();

            GenerateTerrainMesh_Minecraft(sizePerBlock);

            edges.AddComponent<MeshCollider>();
            GameObjectUtility.SetStaticEditorFlags(edges, StaticEditorFlags.BatchingStatic);
            floor.AddComponent<MeshCollider>();
        }
        else
        {
            horBounds = new Vector2Int(posMap.x * chunkSize, (posMap.x * chunkSize) + chunkSize + 1);
            verBounds = new Vector2Int(posMap.y * chunkSize, (posMap.y * chunkSize) + chunkSize + 1);
            LODGroup groups = floor.AddComponent<LODGroup>();
            LOD[] lods = new LOD[3];
            MeshCollider collider = null;
            for (int i = 0; i < lods.Length; i++)
            {
                GameObject child = new GameObject();
                child.transform.SetParent(floor.transform);

                child.AddComponent<MeshRenderer>().material = mat;
                child.AddComponent<MeshFilter>();

                GenerateTerrainMesh_LowPoly(child, mapGenerator.GetMeshSimplificationValue(i));

                if (i == 0) 
                { 
                    collider = child.AddComponent<MeshCollider>(); 
                }


                MeshRenderer[] a = { child.GetComponent<MeshRenderer>() };

                lods[i] = new LOD(1.0F / (i + 2), a);
            }
            
            groups.SetLODs(lods);
            floor.AddComponent<MeshCollider>().sharedMesh = collider.sharedMesh;
            
        }
       
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.BatchingStatic);

        chunk.transform.position = new Vector3(posMap.x * chunkSize * sizePerBlock, 0, -posMap.y * chunkSize * sizePerBlock);
    }

    /// <summary>
    /// Genera la maya del chunk
    /// </summary>
    public void GenerateTerrainMesh_Minecraft(float sizePerBlock)
    {
        int sizeAntiguo = horBounds.y - horBounds.x;
        float[,] matrizReducida = new float[sizeAntiguo - 2, sizeAntiguo - 2];

        // Copiar los elementos relevantes a la nueva matriz
        for (int i = 1; i < sizeAntiguo - 1; i++)
            Array.Copy(generator.Map.HeightMap, i * sizeAntiguo + 1, matrizReducida, (i - 1) * (sizeAntiguo - 2), sizeAntiguo - 2);


        MeshGenerator.GenerateTerrainMeshChunk(generator.Map, floor, sizePerBlock, horBounds, verBounds);
        var copyHorBounds = horBounds;
        var copyVerBounds = verBounds;
        MeshGenerator.DrawEdgesChunk(generator.Map, edges, sizePerBlock, copyHorBounds, copyVerBounds);
    }

    /// <summary>
    /// Genera la maya del chunk
    /// </summary>
    public void GenerateTerrainMesh_LowPoly(GameObject o, int levelOfDetail)
    {
        MeshGenerator.GenerateTerrainMeshChunk_LowPoly(generator.Map, o, levelOfDetail, horBounds, verBounds);
    }

    public void setParent(Transform parent)
    {
        if (chunk != null)
            chunk.transform.SetParent(parent);
    }

    public void delete()
    {
        GameObject.Destroy(chunk.gameObject);
        if(edges!=null)
            GameObject.Destroy(edges.gameObject);
        GameObject.Destroy(objectsGenerated.gameObject);
        GameObject.Destroy(floor.gameObject);
    }

    public void Update(Vector2 playerPos, float maxViewDst)
    {
        Vector2 pos = new Vector2(playerPos.x, -playerPos.y);
        float viewerDstFromNearestEdge = Mathf.Sqrt(bound.SqrDistance(pos));
        SetVisible(viewerDstFromNearestEdge <= maxViewDst);
    }

    public void SetVisible(bool visible)
    {
        chunk.gameObject.SetActive(visible);
    }

    public bool isVisible()
    {
        return chunk.activeSelf;
    }
}