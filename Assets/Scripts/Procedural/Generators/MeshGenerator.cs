using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generador de Mallas
/// </summary>
public static class MeshGenerator
{

    /// <summary>
    /// Genera el suelo del Chunk
    /// </summary>
    public static void GenerateTerrainMeshChunk(MapInfo map, GameObject chunkObject, float sizePerBlock, Vector2Int horBounds, Vector2Int verBounds)
    {
        int size = horBounds.y - horBounds.x;

        float[,] chunkHeightMap = map.HeightMap;

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        Mesh BaseMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura

        for (int relativeY = 0; relativeY < size; relativeY++)
        {
            int y = relativeY + verBounds.x;
            for (int relativeX = 0; relativeX < size; relativeX++)
            {
                int x = relativeX + horBounds.x;

                float height = chunkHeightMap[x, y];
                //definir los vertices de la celda
                Vector3 a = new Vector3(topLeftX + relativeX * sizePerBlock, height, topLeftZ - relativeY * sizePerBlock);
                Vector3 b = new Vector3(topLeftX + (relativeX + 1) * sizePerBlock, height, topLeftZ - relativeY * sizePerBlock);
                Vector3 c = new Vector3(topLeftX + relativeX * sizePerBlock, height, topLeftZ - (relativeY + 1) * sizePerBlock);
                Vector3 d = new Vector3(topLeftX + (relativeX + 1) * sizePerBlock, height, topLeftZ - (relativeY + 1) * sizePerBlock);

                Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                for (int k = 0; k < 6; k++)
                { //crear los triangulos                 
                    vertices.Add(v[k]);
                    triangles.Add(triangles.Count);
                    uvs.Add(uv[k]);
                }

            }
        }

        BaseMesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        BaseMesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        BaseMesh.uv = uvs.ToArray();
        BaseMesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente

        MeshFilter meshFilter = chunkObject.GetComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = BaseMesh;

        var renderer = chunkObject.GetComponent<MeshRenderer>();
        DrawTextureChunk(map, renderer, horBounds, verBounds);
    }

    /// <summary>
    /// Estableze el color de la cada pixel de la textura
    /// </summary>
    public static void DrawTextureChunk(MapInfo map, MeshRenderer renderer, Vector2Int horBounds, Vector2Int verBounds)
    {
        int size = horBounds.y - horBounds.x;

        Texture2D texture = new Texture2D(size, size);
        for (int y = verBounds.x; y < verBounds.y; y++)
        {
            for (int x = horBounds.x; x < horBounds.y; x++)
            {
                Color result = map.GetColorAt(x, y);
                texture.SetPixel(x - size, y - size, result);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        renderer.sharedMaterial.mainTexture = texture;
    }

    /// <summary>
    /// Genera y renderiza los bordes entre las celdas del terreno,(SOLO AQUELLAS Q VAN A SER VISIBLES)
    /// </summary>
    public static void DrawEdgesChunk(MapInfo map, GameObject edges, float sizePerBlock, Vector2Int horBounds, Vector2Int verBounds)
    {
        int size = horBounds.y - horBounds.x;

        float[,] heightMap = map.HeightMap;
        float[,] noiseMap = map.NoiseMap;

        float topLeftX = (size - 1f) / -2f;
        float topLeftZ = (size - 1f) / 2f;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura
        for (int y = 0; y < size; y++)
        {
            int globalY = y + verBounds.x; 
            for (int x = 0; x < size; x++)
            {
                int globalX = x + horBounds.x;
                float height = heightMap[globalX, globalY];

                if (globalX > 0 && globalY >= 0 && globalY < map.Size && globalX < map.Size)
                {
                    float leftHeight = heightMap[globalX - 1, globalY];//izquierda

                    if (leftHeight < height)
                    {
                        float diffHeight = height - leftHeight;

                        //definir los vertices de la celda
                        Vector3 a = new Vector3(topLeftX + x * sizePerBlock, height, topLeftZ - y * sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x * sizePerBlock, height - diffHeight, topLeftZ - y * sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x * sizePerBlock, height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 0; k < 6; k++)
                        {
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (globalX < map.Size - 1 && globalY >= 0 && globalY < map.Size && globalX >= 0)
                {
                    float rightHeight = heightMap[globalX + 1, globalY];//derecha

                    if (rightHeight < height)
                    {
                        float diffHeight = height - rightHeight;

                        Vector3 a = new Vector3(topLeftX + (x + 1) * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + (x + 1) * sizePerBlock, height, topLeftZ - y * sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + (x + 1) * sizePerBlock, height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + (x + 1) * sizePerBlock, height - diffHeight, topLeftZ - y * sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 0; k < 6; k++)
                        {
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (globalY > 0 && globalX >= 0 && globalX < map.Size && globalY < map.Size)
                {
                    float downHeight = heightMap[globalX, globalY - 1];//abajo

                    if (downHeight < height)
                    {
                        float diffHeight = height - downHeight;

                        Vector3 a = new Vector3(topLeftX + x * sizePerBlock, height, topLeftZ - y * sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + (x + 1) * sizePerBlock, height, topLeftZ - y * sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x * sizePerBlock, height - diffHeight, topLeftZ - y * sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + (x + 1) * sizePerBlock, height - diffHeight, topLeftZ - y * sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 5; k >= 0; k--)
                        {// INSERTARLOS AL REVES PQ SI NO SE INVIERTEN LAS NORMALES
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (globalY < map.Size - 1 && globalX >= 0 && globalX < map.Size && globalY >= 0)
                {
                    float upHeight = heightMap[globalX, globalY + 1];//arriba

                    if (upHeight < height)
                    {
                        float diffHeight = height - upHeight;

                        Vector3 a = new Vector3(topLeftX + (x + 1) * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + (x + 1) * sizePerBlock, height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x * sizePerBlock,height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 5; k >= 0; k--)
                        {// INSERTARLOS AL REVES PQ SI NO SE INVIERTEN LAS NORMALES
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        mesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente             

        MeshFilter meshFilter = edges.GetComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = mesh;

        var render = edges.GetComponent<MeshRenderer>();
        DrawTextureChunk(map, render, horBounds, verBounds);
    }


    public static void AddTriangle(List<int> triangles, ref int triangleIndex, int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }


    public static void GenerateTerrainMeshChunk_LowPoly(MapInfo map, GameObject chunkObject, int levelOfDetails, Vector2Int horBounds, Vector2Int verBounds)
    {
        float[,] height = map.HeightMap;

        int sizeX = horBounds.y - horBounds.x;
        int sizeY = horBounds.y - horBounds.x;

        if (verBounds.y < height.GetLength(1))
            sizeY++;

        if (horBounds.y < height.GetLength(0))
            sizeX++;

        float topLeftX = (sizeX - 1) / -2f;
        float topLeftZ = (sizeY - 1) / 2f;

        int meshSimplificationIncrement = levelOfDetails;

        int currentChunkSizeX = sizeX / meshSimplificationIncrement;
        int currentChunkSizeY = sizeY / meshSimplificationIncrement;

        if (verBounds.y < height.GetLength(1))
            currentChunkSizeY++;

        if (horBounds.y < height.GetLength(0))
            currentChunkSizeX++;

        int triangle = (sizeX - 1) * (sizeY - 1) * 6;

        Mesh BaseMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>(new Vector3[currentChunkSizeX * currentChunkSizeY]);//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>(new int[triangle]);
        List<Vector2> uvs = new List<Vector2>(new Vector2[currentChunkSizeX * currentChunkSizeY]); //coordenadas de textura

        int vertexIndex = 0;
        int triangleIndex = 0;

        int y = 0, x = 0;

        for (int relY = 0; relY <= sizeY; relY += meshSimplificationIncrement)
        {
            y = relY + verBounds.x;
            if (y >= height.GetLength(1)) { continue; }

            for (int relX = 0; relX <= sizeX; relX += meshSimplificationIncrement)
            {
                x = relX + horBounds.x;
                if (x >= height.GetLength(0)) { continue; }

                vertices[vertexIndex] = new Vector3(topLeftX + (x - horBounds.x), height[x, y], topLeftZ - (y - verBounds.x));
                uvs[vertexIndex] = new Vector2(x / (float)sizeX, y / (float)sizeY);

                if (x < horBounds.y - meshSimplificationIncrement && y < verBounds.y - meshSimplificationIncrement)
                {
                    AddTriangle(triangles, ref triangleIndex, vertexIndex, vertexIndex + currentChunkSizeX + 1, vertexIndex + currentChunkSizeX);
                    AddTriangle(triangles, ref triangleIndex, vertexIndex + currentChunkSizeX + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        BaseMesh.vertices = vertices.ToArray();// se le asignan los vertices a la malla q hemos creado
        BaseMesh.triangles = triangles.ToArray();// y los triangulos q forman dichos vertices
        BaseMesh.uv = uvs.ToArray();
        BaseMesh.RecalculateNormals();//para que la iluminación y sombreado en la malla se calculen correctamente

        MeshFilter meshFilter = chunkObject.GetComponent<MeshFilter>();//Renderizar la malla
        meshFilter.mesh = BaseMesh;

        var renderer = chunkObject.GetComponent<MeshRenderer>();
        DrawTextureChunk(map, renderer, horBounds, verBounds);
    }
}