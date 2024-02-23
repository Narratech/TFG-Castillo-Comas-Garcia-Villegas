using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public int riversNumAprox = 10;
    public int riverLength = 150;
    public bool bold = true;
    public bool converganceOn = true;

    MapGenerator mapGenerator;
    float[,] noise;

    void Start(){
        mapGenerator = GetComponent<MapGenerator>();
        if ( mapGenerator == null ) 
            mapGenerator = gameObject.AddComponent<MapGenerator>();
    }

    bool IsLower(int neighbourX,int neighbourY,float currentHeight)
    {
        if (neighbourX >= 0 && neighbourX <noise.GetLength(0) && neighbourY >= 0 && neighbourY < noise.GetLength(1))
            return noise[neighbourX,neighbourY] < currentHeight;

        return false;
    }
    /// <summary>
    /// Comprobar que las casillas alrededor hay alguna mas baja que el resto
    /// </summary>
    /// <param name="place">Casilla a comprobar</param>
    /// <returns></returns>
    bool avaliableToCreate(Vector2Int place)
    {
        var currentHeight = noise[place.x, place.y];
        if (IsLower(place.x-1,place.y,currentHeight) ||
            IsLower(place.x + 1, place.y, currentHeight) ||
            IsLower(place.x, place.y - 1, currentHeight) ||
            IsLower(place.x, place.y + 1, currentHeight))
            return true;
        return false;
    }

    /// <summary>
    /// Cribar las casillas de maxima altura del mapa de cara a las casillas que no tienenotras alrededor que le superen en altura
    /// </summary>
    /// <param name="toCreate"></param>
    /// <returns></returns>
    List<Vector2Int> BestPlacesToCreate (List<Vector2Int> toCreate){
        List<Vector2Int> bestPlaces = new List<Vector2Int>();

        foreach (Vector2Int place in toCreate)
        {
            if (avaliableToCreate(place))
                bestPlaces.Add(place);
        }

        return bestPlaces;
    }

    public float[,] GenerateRivers(Cell[,] cells)
    {
        mapGenerator = GetComponent<MapGenerator>();
        noise = mapGenerator.getNoise();
        
        var result = Noise.FindLocalMaxima(noise); //maximas
        var toCreate = result.Where(pos => noise[pos.x, pos.y] > mapGenerator.regions[mapGenerator.regions.Length -2].height).OrderBy(a => Guid.NewGuid()).Take(riversNumAprox).ToList();
        toCreate = BestPlacesToCreate(toCreate); //Hacer otra criba con aquellas casillas que alrededor no tienen otras de height inferior
        var waterMinimas = Noise.FindLocalMinima(noise);
        
        waterMinimas = waterMinimas.Where(pos => noise[pos.x, pos.y] < mapGenerator.regions[1].height).OrderBy(pos => noise[pos.x, pos.y]).Take(riversNumAprox * 2).ToList();
        
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

        var position = RiverGrowth(worm.MoveLength(riverLength), startPosition);
        PlaceRiverTile(position,startPosition);
    }

    void PlaceRiverTile(List<Vector2> positons, Vector2 startPosition)
    {
        foreach (var pos in positons)
        {
            //cells[(int)pos.x, (int)pos.y].type.color = Color.blue;
            if (pos.x < mapGenerator.mapSize && pos.y < mapGenerator.mapSize && pos.x >= 0 && pos.y >= 0)
            {
                noise[(int)pos.x, (int)pos.y] = 0.15f;

                if (bold && noise[(int)pos.x, (int)pos.y] < 0.9f)
                {
                    //cells[(int)pos.x+1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x-1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y+1].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y-1].type.color = Color.blue;
                    if (pos.x + 1 < mapGenerator.mapSize )
                        noise[(int)pos.x + 1, (int)pos.y] = 0.15f;
                    if (pos.x - 1 >= 0 )
                        noise[(int)pos.x - 1, (int)pos.y] = 0.15f;
                    if (pos.y + 1 < mapGenerator.mapSize )
                        noise[(int)pos.x, (int)pos.y + 1] = 0.15f;
                    if (pos.y - 1 >= 0 )
                        noise[(int)pos.x, (int)pos.y - 1] = 0.15f;
                }
            }
        } 
    }

    bool CheckRiverGrowth(Vector2 currentPos, Vector2 candidatePosition){
        return noise[(int)currentPos.x, (int)currentPos.y] > noise[(int)candidatePosition.x, (int)candidatePosition.y];
    }

    bool casillaValida(Vector2 pos)
    {
        return pos.x < mapGenerator.mapSize && pos.y < mapGenerator.mapSize && pos.x >= 0 && pos.y >= 0;
    }

    private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> parents, Vector2 goal)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 current = goal;

        while (current != null)
        {
            path.Add(current);
            current = parents[current];
        }

        path.Reverse();
        return path;
    }

    private List<Vector2> GetNeighbors(Vector2 node)
    {
        List<Vector2> neighbors = new List<Vector2>();
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        int size = noise.GetLength(0);
        foreach (var dir in directions)
        {
            int newX = (int)(node.x + dir.x);
            int newY = (int)(node.y + dir.y);
            Vector2 neighbor = new Vector2(newX, newY);
           if (casillaValida(neighbor) && CheckRiverGrowth(node, neighbor)) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    List<Vector2> searchCamino(Vector2 start, Vector2 objetive)
    {
        Queue<Vector2> queue = new Queue<Vector2>();
        HashSet<Vector2> visited = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();

        queue.Enqueue(start);
        visited.Add(start);
        parents[start] = start;

        while (queue.Count > 0)
        {
            Vector2 node = queue.Dequeue();

            if (CheckRiverGrowth(objetive, node) && node.x == objetive.x && node.y == objetive.y)
            {
                return ReconstructPath(parents, objetive);
            }

            foreach (Vector2 neighbor in GetNeighbors(node))
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parents[neighbor] = node;
                }
            }
        }

        return null; // No se encontró ningún camino
    }

    List<Vector2> RiverGrowth(List<Vector2> positions,Vector2 startPosition){
        List<Vector2> riverWay = new List<Vector2>();
        Vector2 last = startPosition;int contador = 0;
        for (int i= 0; i < positions.Count; i++){
            Vector2 pos = positions[i];
            if (casillaValida(pos))
            {
                if (!CheckRiverGrowth(last, pos) && contador+1 < positions.Count && casillaValida(positions[contador + 1])) // mirar donde puede continuar el rio
                {
                    var camino = searchCamino(last, positions[contador+1]);
                    if (camino != null){
                        riverWay.AddRange(camino);
                        last = positions[contador +1];
                        contador ++;
                        
                    }
                    else contador += 2;
                }
                else{
                    riverWay.Add(pos);
                    last = pos;
                }
            }
            contador++;
        }
        return riverWay;
    }
}
