using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 100; // Distacia maxima a la que ve el jugador
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
            chunkVisibleLastUpdate[i].SetVisible(false);
        }
        chunkVisibleLastUpdate.Clear();

        int currentChunkCoordsX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordsY = Mathf.RoundToInt(-playerPos.y / chunkSize);

        for (int yOffset = -chunksVisibleViewDst; yOffset <= chunksVisibleViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleViewDst; xOffset <= chunksVisibleViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2((int)(currentChunkCoordsX + xOffset), (int)(currentChunkCoordsY + yOffset));
                if(viewedChunkCoord.x >=0 && viewedChunkCoord.y >= 0)
                {

                    if (map3D.ContainsKey(viewedChunkCoord))
                    {
                        //mirar si esta visible y si no lo esta hacerlo visible
                        map3D[viewedChunkCoord].Update(playerPos, maxViewDst);
                        if (map3D[viewedChunkCoord].isVisible())
                        {
                            chunkVisibleLastUpdate.Add(map3D[viewedChunkCoord]);
                        }
                    }
                    else
                    {
                        //llamar a mapgenerator para k genere ese chunk
                        map3D.Add(viewedChunkCoord, new Chunk(mapGenerator, new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y), mapGenerator.sizePerBlock, mapGenerator.chunkSize, mapGenerator.gameObjectMap3D.transform, false, 0));

                    }
                }
                
            }
        }
        Debug.Log("Chunk Player Position: " + currentChunkCoordsY + " " + currentChunkCoordsX);
    }

    private void Update()
    {
        playerPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        UpdateVisibleChunks();
    }

}
