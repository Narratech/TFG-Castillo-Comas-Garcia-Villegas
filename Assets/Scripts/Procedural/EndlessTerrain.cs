using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 200; // Distacia maxima a la que ve el jugador
    public Transform playerTransform; //Posicion del player

    public static Vector2 playerPos; //Static para acceder desde otras clases mejor
    int chunkSize;
    int chunksVisibleViewDst; //Numero de chunks visibles
    MapGenerator mapGenerator;
   
    List<Chunk> chunkVisibleLastUpdate = new List<Chunk>();
    public Dictionary<Vector2, Chunk> map3D = new Dictionary<Vector2, Chunk>();
    private void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        
        //viewer.position = new Vector3(0, 0, -mapGenerator.  / chunkSize);
        mapGenerator.GenerateEndlessMap();
        chunkSize = mapGenerator.chunkSize - 1;
        chunksVisibleViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < chunkVisibleLastUpdate.Count; i++)
        {
            chunkVisibleLastUpdate[i].delete();
        }
        chunkVisibleLastUpdate.Clear();

        int currentChunkCoordsX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordsY = Mathf.RoundToInt(-playerPos.y / chunkSize);

        for (int yOffset = -chunksVisibleViewDst; yOffset <= chunksVisibleViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleViewDst; xOffset <= chunksVisibleViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2((int)(currentChunkCoordsX + xOffset), (int)(currentChunkCoordsY + yOffset));
                if(viewedChunkCoord.x >=0 && viewedChunkCoord.y >= 0 && viewedChunkCoord.x< mapGenerator.mapSize/chunkSize && viewedChunkCoord.y < mapGenerator.mapSize / chunkSize)
                {
                    int LOD = 1;
                    float dist = Vector3.Distance(viewedChunkCoord, new Vector2(currentChunkCoordsX, currentChunkCoordsY));
                    if (dist > 3) 
                        LOD = 49;

                    if (map3D.ContainsKey(viewedChunkCoord))
                    {
                        //mirar si esta visible y si no lo esta hacerlo visible
                        float viewerDstFromNearestEdge = map3D[viewedChunkCoord].Update(playerPos);
                        if (viewerDstFromNearestEdge <= maxViewDst)
                        {
                            map3D[viewedChunkCoord].delete();
                            map3D.Remove(viewedChunkCoord);
                            map3D[viewedChunkCoord] =
                            new Chunk(
                                mapGenerator,
                                new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y),
                                mapGenerator.sizePerBlock,
                                mapGenerator.chunkSize,
                                mapGenerator.gameObjectMap3D.transform,
                                mapGenerator.drawMode == MapGenerator.DrawMode.Cartoon ? true : false,
                                LOD);
                                chunkVisibleLastUpdate.Add(map3D[viewedChunkCoord]);
                        }
                        else 
                        {
                            map3D[viewedChunkCoord].delete();
                            //map3D.Remove(viewedChunkCoord);
                        }
                    }
                    else
                    {
                        //llamar a mapgenerator para k genere ese chunk
                        map3D.Add(viewedChunkCoord, 
                            new Chunk(
                                mapGenerator, 
                                new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y), 
                                mapGenerator.sizePerBlock, 
                                mapGenerator.chunkSize, 
                                mapGenerator.gameObjectMap3D.transform, 
                                mapGenerator.drawMode == MapGenerator.DrawMode.Cartoon ? true : false,
                                LOD));

                    }
                }
                
            }
        }
    }

    private void Update()
    {
        playerPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        UpdateVisibleChunks();
    }

}
