using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Capa de terreno que se puede generar
/// </summary>
[System.Serializable]
public struct TerrainType{
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
public class ObjectInMap{
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
public class Chunk{
    /// <summary>
    /// Posicion del chunk en el mapa
    /// </summary>
    public Vector2 posMap;

    GameObject chunk;
    GameObject floor;
    GameObject edges;
    public GameObject objectsGenerated;

    public Chunk(Vector2 posMap, Cell[,] mapaCells, float heightPerBlock,float sizePerBlock, int chunkSize, Transform parent){
        this.posMap = posMap;

        //Generamos los GameObjects
        chunk = new GameObject("Chunk " + posMap);
        floor = new GameObject("Suelo " + posMap);
        edges = new GameObject("Edges " + posMap);
        objectsGenerated = new GameObject("Objectos " + posMap);

        //Establecemos la jerarquia de padres
        setParent(parent);
        floor.transform.SetParent(chunk.transform);
        edges.transform.SetParent(chunk.transform);
        objectsGenerated.transform.SetParent(chunk.transform);

        //Creamos los respectivos materiales para cada malla
        Material sueloMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floor.AddComponent<MeshFilter>();
        floor.AddComponent<MeshRenderer>().material = sueloMaterial;

        Material edgesMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        edges.AddComponent<MeshFilter>();
        edges.AddComponent<MeshRenderer>().material = edgesMaterial;

        //Generamos la maya
        GenerateTerrainMesh(mapaCells,sizePerBlock,chunkSize);
        
        floor.AddComponent<MeshCollider>();
        edges.AddComponent<MeshCollider>();

        //GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.BatchingStatic);
        //GameObjectUtility.SetStaticEditorFlags(edges, StaticEditorFlags.BatchingStatic);
    }

    /// <summary>
    /// Genera la maya del chunk
    /// </summary>
    public void GenerateTerrainMesh(Cell[,] mapaCells,float sizePerBlock,int chunkSize){
        MeshGenerator.GenerateTerrainMeshChunk(mapaCells, posMap, floor, sizePerBlock,chunkSize);
        MeshGenerator.DrawEdgesChunk(mapaCells, posMap, edges, sizePerBlock,chunkSize);
    }

    public void setParent(Transform parent){
        chunk.transform.SetParent(parent);
    }

    public void delete(){
        GameObject.Destroy(chunk.gameObject);
        GameObject.Destroy(edges.gameObject);
        GameObject.Destroy(objectsGenerated.gameObject);
        GameObject.Destroy(floor.gameObject);
    }
}
