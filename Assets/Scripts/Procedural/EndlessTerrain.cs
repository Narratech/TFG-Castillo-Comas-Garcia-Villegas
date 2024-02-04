using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDst = 300;
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunksVisibleViewDst;
    MapGenerator mapGenerator;
    private void Start(){
        mapGenerator = GetComponent<MapGenerator>();
        chunkSize = mapGenerator.chunkSize - 1;
        chunksVisibleViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    void UpdateVisibleChunks(){
        int currentChunkCoordsX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordsY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleViewDst; yOffset <= chunksVisibleViewDst; yOffset++){
            for (int xOffset = -chunksVisibleViewDst; xOffset <= chunksVisibleViewDst; xOffset++){
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordsX+xOffset, currentChunkCoordsY+yOffset);

                if (mapGenerator.Map3D.ContainsKey(viewedChunkCoord)){
                    //mirar si esta visible y si no lo esta hacerlo visible
                }
                else{
                   //llamar a mapgenerator para k genere ese chunk
                }

            }
        }
    }
}
