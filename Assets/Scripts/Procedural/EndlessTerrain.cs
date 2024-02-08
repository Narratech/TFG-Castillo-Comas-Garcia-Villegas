using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        viewer.position = new Vector3(0, 0, -mapGenerator.mapSize/chunkSize);
    }

    void UpdateVisibleChunks(){
        int currentChunkCoordsX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordsY = Mathf.RoundToInt(viewerPosition.y / chunkSize)+1;

        for (int yOffset = -chunksVisibleViewDst; yOffset <= chunksVisibleViewDst; yOffset++){
            for (int xOffset = -chunksVisibleViewDst; xOffset <= chunksVisibleViewDst; xOffset++){
                Vector2 viewedChunkCoord = new Vector2((int)(currentChunkCoordsX+xOffset), (int)(currentChunkCoordsY+yOffset));

                if (mapGenerator.Map3D.ContainsKey(new Vector2(viewedChunkCoord.x, viewedChunkCoord.y))){
                    //mirar si esta visible y si no lo esta hacerlo visible
                    mapGenerator.Map3D[viewedChunkCoord].Update(viewerPosition,maxViewDst);
                }
                else{
                    //llamar a mapgenerator para k genere ese chunk
                    //mapGenerator.Map3D.Add(viewedChunkCoord, new Chunk(mapGenerator,new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y),0.5f,chunkSize,transform,false,0));
                }

            }
        }
        Debug.Log("Chunk Player Position: " + currentChunkCoordsY + " " + currentChunkCoordsX);
    }

    private void Update(){
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }
}
