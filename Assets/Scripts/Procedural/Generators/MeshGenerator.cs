using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generador de Mallas
/// </summary>
public static class MeshGenerator{

    /// <summary>
    /// Genera el suelo del Chunk
    /// </summary>
    public static void GenerateTerrainMeshChunk(Cell[,] mapaCells, Vector2 pos, GameObject chunkObject, float sizePerBlock, int chunkSize){
        int size = mapaCells.GetLength(0);

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / -2f;
     
        Mesh BaseMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();//Almacenar los vertices y triangulos de la malla
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura

        for (int y = (int)pos.y * chunkSize; y < (int)(pos.y + 1) * chunkSize && y < size; y++){
            for (int x = (int)pos.x*chunkSize; x < (int)(pos.x+1)*chunkSize && x<size; x++){
                Cell cell = mapaCells[x, y];
                //definir los vertices de la celda
                Vector3 a = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                Vector3 b = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                Vector3 c = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);
                Vector3 d = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);

                Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                for (int k = 0; k < 6; k++){ //crear los triangulos                 
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
        DrawTextureChunk(mapaCells,pos,renderer,chunkSize);
    }

    /// <summary>
    /// Estableze el color de la cada pixel de la textura
    /// </summary>
    public static void DrawTextureChunk(Cell[,] mapaCells, Vector2 pos, MeshRenderer renderer, int chunkSize){
        int size = mapaCells.GetLength(0);

        Texture2D texture = new Texture2D(size, size);
        for (int y = (int)pos.y * chunkSize; y < (int)(pos.y + 1) * chunkSize && y < size; y++){
            for (int x = (int)pos.x * chunkSize; x < (int)(pos.x + 1) * chunkSize && x < size; x++){
                Cell cell = mapaCells[x, y];
                texture.SetPixel(x, y, cell.type.color);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        renderer.sharedMaterial.mainTexture = texture;
    }

    /// <summary>
    /// Genera y renderiza los bordes entre las celdas del terreno,(SOLO AQUELLAS Q VAN A SER VISIBLES)
    /// </summary>
    public static void DrawEdgesChunk(Cell[,] mapaCells, Vector2 pos, GameObject edges,float sizePerBlock,int chunkSize){
        int size = mapaCells.GetLength(0);

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / -2f;

        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>(); //coordenadas de textura
        for (int y = (int)pos.y * chunkSize; y < (int)(pos.y + 1) * chunkSize && y < size; y++){
            for (int x = (int)pos.x * chunkSize; x < (int)(pos.x + 1) * chunkSize && x < size; x++){
                Cell cell = mapaCells[x, y];               

                if (x > 0){
                    Cell left = mapaCells[x - 1, y];//izquierda
                    if (left.noise < cell.noise){
                        float leftHeight = cell.Height - left.Height;
                        Vector3 a = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x - sizePerBlock, cell.Height - leftHeight, topLeftZ - y + sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x - sizePerBlock, cell.Height - leftHeight, topLeftZ - y - sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 0; k < 6; k++){
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (x < size - 1){
                    Cell right = mapaCells[x + 1, y];//derecha
                    if (right.noise < cell.noise){
                        float rightHeight = cell.Height - right.Height;
                        Vector3 a = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x + sizePerBlock, cell.Height - rightHeight, topLeftZ - y - sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x + sizePerBlock, cell.Height - rightHeight, topLeftZ - y + sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 0; k < 6; k++){
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (y > 0){
                    Cell down = mapaCells[x, y - 1];//abajo
                    if (down.noise < cell.noise){
                        float downHeight = cell.Height - down.Height;
                        Vector3 a = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y + sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x - sizePerBlock, cell.Height - downHeight, topLeftZ - y + sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x + sizePerBlock, cell.Height - downHeight, topLeftZ - y + sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 5; k >= 0; k--){// INSERTARLOS AL REVES PQ SI NO SE INVIERTEN LAS NORMALES
                            vertices.Add(v[k]);
                            triangles.Add(triangles.Count);
                            uvs.Add(uv[k]);
                        }

                    }
                }
                if (y < size - 1){
                    Cell up = mapaCells[x, y + 1];//arriba
                    if (up.noise < cell.noise){
                        float upHeight = cell.Height - up.Height;
                        Vector3 a = new Vector3(topLeftX + x + sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);
                        Vector3 b = new Vector3(topLeftX + x - sizePerBlock, cell.Height, topLeftZ - y - sizePerBlock);
                        Vector3 c = new Vector3(topLeftX + x + sizePerBlock, cell.Height - upHeight, topLeftZ - y - sizePerBlock);
                        Vector3 d = new Vector3(topLeftX + x - sizePerBlock, cell.Height - upHeight, topLeftZ - y - sizePerBlock);

                        Vector2 uvA = new Vector2(x / (float)size, y / (float)size); //definir coordenadas de textura correspondientes a cada v
                        Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                        Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                        Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);

                        Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                        Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                        for (int k = 5; k >= 0; k--){// INSERTARLOS AL REVES PQ SI NO SE INVIERTEN LAS NORMALES
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
        DrawTextureChunk(mapaCells,pos,render, chunkSize);
    }
}