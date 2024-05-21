using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    [Tooltip("Distancia maxima visible por el jugador")]
    [SerializeField] float maxViewDst = 200; // Distacia maxima a la que ve el jugador
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
                if(viewedChunkCoord.x >=0 && viewedChunkCoord.y >= 0 && viewedChunkCoord.x< mapGenerator.mapSize/chunkSize && viewedChunkCoord.y < mapGenerator.mapSize / chunkSize)
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
                        map3D.Add(viewedChunkCoord, 
                            new Chunk(
                                mapGenerator, 
                                new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y), 
                                mapGenerator.sizePerBlock, 
                                mapGenerator.chunkSize, 
                                mapGenerator.gameObjectMap3D.transform,
                                mapGenerator.material)
                               );
                        //GENERAR LOS OBJECTOS DEL CHUNK
                        if (mapGenerator.generateObjects)
                        {
                            StartCoroutine(ObjectsGenerator.GenerateObjectsEndLess(mapGenerator.Map,
                                mapGenerator.GetBiomeGenerator(),
                                new Vector2Int((int)viewedChunkCoord.x, (int)viewedChunkCoord.y),
                                map3D[viewedChunkCoord],
                                mapGenerator.drawMode == MapGenerator.DrawMode.Cartoon ? mapGenerator.mapSize - 1 : mapGenerator.mapSize
                                ));
                            //Debug.Log("Generar Objectos Chunk");
                        }
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
