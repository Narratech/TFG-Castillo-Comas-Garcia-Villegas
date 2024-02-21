using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    MapGenerator mapGenerator;
    Vector2 riverStartPosition;
    int riverLength = 50;
    public bool bold = true;
    public bool converganceOn = true;

    float[,] noise;
    void Start(){
        mapGenerator = GetComponent<MapGenerator>();
        if ( mapGenerator == null ) 
            mapGenerator = gameObject.AddComponent<MapGenerator>();
    }
    public float[,] GenerateRivers(Cell[,] cells)
    {
        mapGenerator = GetComponent<MapGenerator>();
        noise = mapGenerator.getNoise();
        
        var result = Noise.FindLocalMaxima(noise); //maximas
        //if ( result.Count < 1) return noise;
        
        var toCreate = result.Where(pos => noise[pos.x, pos.y] > mapGenerator.regions[mapGenerator.regions.Length -2].height).OrderBy(a => Guid.NewGuid()).Take(5).ToList();
       
        var waterMinimas = Noise.FindLocalMinima(noise);
        //if (waterMinimas.Count < 1) return noise;
        waterMinimas = waterMinimas.Where(pos => noise[pos.x, pos.y] < mapGenerator.regions[1].height).OrderBy(pos => noise[pos.x, pos.y]).Take(20).ToList();
        
        foreach (var item in toCreate)
            CreateRiver(item,cells, waterMinimas);
        return noise;
    }

    private void CreateRiver(Vector2Int startPosition,Cell[,] cells, List<Vector2Int> waterMinimas)
    {
        PerlinWorm worm;
        if (converganceOn)
        {
            var closestWaterPos = waterMinimas.OrderBy(pos => Vector2.Distance(pos, startPosition)).First();
            worm = new PerlinWorm(noise[startPosition.x,startPosition.y], startPosition, closestWaterPos);
        }
        else
        {
            worm = new PerlinWorm(noise[startPosition.x, startPosition.y], startPosition);
        }

        var position = worm.MoveLength(riverLength);
        PlaceRiverTile(position,cells);

    }

    //IEnumerator PlaceRiverTile(List<Vector2> positons, Cell[,] cells)
    //{
    //    foreach (var pos in positons)
    //    {
    //        //cells[(int)pos.x, (int)pos.y].type.color = Color.blue;
    //        noise[(int)pos.x, (int)pos.y] = 0.2f;
    //        if (bold &&noise[(int)pos.x, (int)pos.y] < 0.9f/*mapGenerator.hillHeight*/)
    //        {
    //            //cells[(int)pos.x+1, (int)pos.y].type.color = Color.blue;
    //            //cells[(int)pos.x-1, (int)pos.y].type.color = Color.blue;
    //            //cells[(int)pos.x, (int)pos.y+1].type.color = Color.blue;
    //            //cells[(int)pos.x, (int)pos.y-1].type.color = Color.blue;

    //            noise[(int)pos.x+1, (int)pos.y] = 0.2f;
    //            noise[(int)pos.x-1, (int)pos.y] = 0.2f;
    //            noise[(int)pos.x, (int)pos.y+1] = 0.2f;
    //            noise[(int)pos.x, (int)pos.y-1] = 0.2f;
    //        }
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //    yield return null;
    //}

    void PlaceRiverTile(List<Vector2> positons, Cell[,] cells)
    {
        foreach (var pos in positons)
        {
            //Debug.Log(pos);
            //cells[(int)pos.x, (int)pos.y].type.color = Color.blue;
            if (pos.x < mapGenerator.mapSize && pos.y < mapGenerator.mapSize && pos.x >= 0 && pos.y >= 0)
            {
                //Debug.Log(pos + "Pasa");
                noise[(int)pos.x, (int)pos.y] = 0.15f;

                if (bold && noise[(int)pos.x, (int)pos.y] < 0.9f/*mapGenerator.hillHeight*/)
                {
                    //cells[(int)pos.x+1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x-1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y+1].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y-1].type.color = Color.blue;
                    if (pos.x + 1 < mapGenerator.mapSize)
                        noise[(int)pos.x + 1, (int)pos.y] = 0.15f;
                    if (pos.x - 1 >= 0)
                        noise[(int)pos.x - 1, (int)pos.y] = 0.15f;
                    if (pos.y + 1 < mapGenerator.mapSize)
                        noise[(int)pos.x, (int)pos.y + 1] = 0.15f;
                    if (pos.y - 1 >= 0)
                        noise[(int)pos.x, (int)pos.y - 1] = 0.15f;
                }
            }
        } 
    }
}
