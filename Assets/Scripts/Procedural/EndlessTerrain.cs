using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 300; // Distacia maxima a la que ve el jugador
    public Transform playerTransform; //Posicion del player

    public static Vector2 playerPos; //Static para acceder desde otras clases mejor
    int chunkSize;
    int chunksVisibleViewDst; //Numero de chunks visibles
    MapGenerator mapGenerator;
   
    List<Chunk> chunkVisibleLastUpdate = new List<Chunk>();
    private void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        
        chunkSize = mapGenerator.chunkSize - 1;
        chunksVisibleViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        //viewer.position = new Vector3(0, 0, -mapGenerator.  / chunkSize);

    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < chunkVisibleLastUpdate.Count; i++)
        {
            chunkVisibleLastUpdate[i].SetVisible(false);
        }
        chunkVisibleLastUpdate.Clear();

        int currentChunkCoordsX = Mathf.RoundToInt(playerPos.x / chunkSize);
        int currentChunkCoordsY = Mathf.RoundToInt(playerPos.y / chunkSize) /*+ 1*/;

        for (int yOffset = -chunksVisibleViewDst; yOffset <= chunksVisibleViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleViewDst; xOffset <= chunksVisibleViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2((int)(currentChunkCoordsX + xOffset), (int)(currentChunkCoordsY + yOffset));
                
                if (mapGenerator.Map3D.ContainsKey(viewedChunkCoord))
                {
                    //mirar si esta visible y si no lo esta hacerlo visible
                    mapGenerator.Map3D[viewedChunkCoord].Update(playerPos, maxViewDst);
                    if (mapGenerator.Map3D[viewedChunkCoord].isVisible())
                    {
                        chunkVisibleLastUpdate.Add(mapGenerator.Map3D[viewedChunkCoord]);
                    }
                }
                else
                {
                    //llamar a mapgenerator para k genere ese chunk
                    mapGenerator.Map3D[viewedChunkCoord] =
                        new Chunk(mapGenerator,new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y), 0.5f, chunkSize, mapGenerator.gameObjectMap3D.transform,false,0);
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
