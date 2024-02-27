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

                Debug.Log("HeightMap size: " + chunkHeightMap.GetLength(0));
                Debug.Log(horBounds.x + ", " + horBounds.y);
                Debug.Log(x + ", " + y);
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
                float noise = noiseMap[globalX, globalY];
                if (height != null)
                {
                    if (globalX > 0 && globalY >= 0 && globalY < map.Size && globalX < map.Size)
                    {
                        float leftHeight = heightMap[globalX - 1, globalY];//izquierda
                        float leftNoise = noiseMap[globalX - 1, globalY];//izquierda

                        if (leftNoise < noise)
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
                        float rightNoise = noiseMap[globalX + 1, globalY];//derecha

                        if (rightNoise < noise)
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
                        float downNoise = noiseMap[globalX, globalY - 1];//abajo

                        if (downNoise < noise)
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
                        float upNoise = noiseMap[globalX, globalY + 1];//arriba

                        if (upNoise < noise)
                        {
                            float diffHeight = height - upHeight;

                            Vector3 a = new Vector3(topLeftX + (x + 1) * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                            Vector3 b = new Vector3(topLeftX + x * sizePerBlock, height, topLeftZ - (y + 1) * sizePerBlock);
                            Vector3 c = new Vector3(topLeftX + (x + 1) * sizePerBlock, height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);
                            Vector3 d = new Vector3(topLeftX + x * sizePerBlock, height - diffHeight, topLeftZ - (y + 1) * sizePerBlock);

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
        int size = horBounds.y - horBounds.x;
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        int meshSimplificationIncrement = levelOfDetails;
        int currentChunkSize = (int)(((float)size / (float)meshSimplificationIncrement) + 0.9999f);

        int triangle = (size - 1) * (size - 1) * 6;

        Mesh BaseMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>(new Vector3[currentChunkSize * currentChunkSize]);//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>(new int[triangle]);
        List<Vector2> uvs = new List<Vector2>(new Vector2[currentChunkSize * currentChunkSize]); //coordenadas de textura

        int vertexIndex = 0;
        int triangleIndex = 0;

        Debug.Log("currentChunkSize * currentChunkSize: " + currentChunkSize * currentChunkSize);
        Debug.Log("size: " + size);
        Debug.Log("meshSimplificationIncrement: " + meshSimplificationIncrement);
        for (int y = verBounds.x; y < verBounds.y; y += meshSimplificationIncrement)
        {
            for (int x = horBounds.x; x < horBounds.y; x += meshSimplificationIncrement)
            {
                vertices[vertexIndex] = new Vector3(topLeftX + (x - horBounds.x), height[x, y], topLeftZ - (y - verBounds.x));
                uvs[vertexIndex] = new Vector2(x / (float)size, y / (float)size);

                if (x < horBounds.y - meshSimplificationIncrement && y < verBounds.y - meshSimplificationIncrement)
                {
                    AddTriangle(triangles, ref triangleIndex, vertexIndex, vertexIndex + currentChunkSize + 1, vertexIndex + currentChunkSize);
                    AddTriangle(triangles, ref triangleIndex, vertexIndex + currentChunkSize + 1, vertexIndex, vertexIndex + 1);
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