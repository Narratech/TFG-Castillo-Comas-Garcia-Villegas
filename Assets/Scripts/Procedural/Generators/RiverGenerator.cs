using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    public int riversMax = 10;
    public int capaGeneracion = 2;
    public int riverLength = 150;
    public bool bold = true;
    public bool converganceOn = true;

    MapGenerator mapGenerator;
    Cell[,] map;


    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator == null)
            mapGenerator = gameObject.AddComponent<MapGenerator>();
    }

    public Cell[,] GenerateRivers(Cell[,] cells)
    {
        mapGenerator = GetComponent<MapGenerator>();
        map = cells;

        var result = Noise.FindLocalMaxima(cells); //maximas
        var toCreate = result.Where(pos => cells[pos.x, pos.y].noise <= mapGenerator.regions[capaGeneracion].height).
            OrderBy(a => Guid.NewGuid()).Take(UnityEngine.Random.Range(1, riversMax)).ToList();
        var waterMinimas = Noise.FindLocalMinima(cells);

        waterMinimas = waterMinimas.Where(pos => cells[pos.x, pos.y].noise < mapGenerator.regions[1].height).OrderBy(pos => cells[pos.x, pos.y].noise).Take(riversMax * 2).ToList();

        foreach (var item in toCreate)
            CreateRiver(item, cells, waterMinimas);

        return cells;
    }

    private void CreateRiver(Vector2Int startPosition, Cell[,] cells, List<Vector2Int> waterMinimas)
    {
        PerlinWorm worm;
        if (converganceOn)
        {
            var closestWaterPos = waterMinimas.OrderBy(pos => Vector2.Distance(pos, startPosition)).First();
            worm = new PerlinWorm(cells[startPosition.x, startPosition.y].noise, startPosition, closestWaterPos);
        }
        else
        {
            worm = new PerlinWorm(cells[startPosition.x, startPosition.y].noise, startPosition);
        }

        var position = RiverGrowth(worm.MoveLength(riverLength), startPosition);
        PlaceRiverTile(position, startPosition);
    }

    List<Vector2Int> RiverGrowth(List<Vector2> positions, Vector2 startPosition)
    {
        List<Vector2Int> riverWay = new List<Vector2Int>(); //lista del camino del rio
        Vector2Int last = new Vector2Int((int)startPosition.x, (int)startPosition.y);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2Int pos = new Vector2Int((int)positions[i].x, (int)positions[i].y);
            if (casillaValida(pos))
            {
                var portionPath = FindPath(last, pos);
                if (portionPath != null)
                {

                    riverWay.AddRange(portionPath);
                    last = pos;
                }

            }
        }
        return riverWay;
    }

    bool CheckRiverGrowth(Vector2 currentPos, Vector2 candidatePosition)
    {
        return map[(int)currentPos.x, (int)currentPos.y].noise >= map[(int)candidatePosition.x, (int)candidatePosition.y].noise;
    }

    bool casillaValida(Vector2 pos)
    {
        return pos.x < mapGenerator.mapSize && pos.y < mapGenerator.mapSize && pos.x >= 0 && pos.y >= 0;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        //Lista de posibles casillas OPEN
        var hashSet = new HashSet<Vector2Int>();
        hashSet.Add(start);

        //Mjeor diccionario para mantener costos g(N)
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();

        foreach (var pos in getAllPositions())
            gScore[pos] = float.MaxValue;

        gScore[start] = 0; // Desde donde partimos

        //Mantener la informacion de Cada Nodo con Nodo
        Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();

        while (hashSet.Count > 0)
        {
            Vector2Int current = LowestFScore(hashSet, gScore);
            if (current == end)
            {
                return ReconstructPath(parents, current);
            }

            hashSet.Remove(current);

            foreach (var neighbour in GetNeighbors(current)) //miramos vecinos
            {
                float candidateNoise = gScore[current] + map[neighbour.x, neighbour.y].noise;

                if (candidateNoise <= gScore[neighbour])
                {
                    parents[neighbour] = current;
                    gScore[neighbour] = candidateNoise;
                    hashSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    IEnumerable<Vector2Int> getAllPositions()
    {
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                yield return new Vector2Int(i, j);
    }

    Vector2Int LowestFScore(HashSet<Vector2Int> hashSet, Dictionary<Vector2Int, float> gScore)
    {
        Vector2Int minNode = new Vector2Int();
        float minFScore = float.MaxValue;

        foreach (var node in hashSet)
        {
            if (gScore[node] < minFScore)
            {
                minFScore = gScore[node];
                minNode = node;
            }
        }
        return minNode;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            int newX = node.x + dir.x;
            int newY = node.y + dir.y;
            Vector2Int neighbor = new Vector2Int(newX, newY);
            if (casillaValida(neighbor) && CheckRiverGrowth(node, neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> path, Vector2Int current)
    {
        List<Vector2Int> riverPath = new List<Vector2Int>();
        riverPath.Add(current);

        while (path.ContainsKey(current)) //reconstruirmos del reves
        {
            current = path[current];
            riverPath.Add(current);
        }

        riverPath.Reverse(); //damos la vuelta
        return riverPath;
    }










    void PlaceRiverTile(List<Vector2Int> positons, Vector2 startPosition)
    {
        foreach (var pos in positons)
        {
            //cells[(int)pos.x, (int)pos.y].type.color = Color.blue;
            if (pos.x < mapGenerator.mapSize && pos.y < mapGenerator.mapSize && pos.x >= 0 && pos.y >= 0)
            {
                map[(int)pos.x, (int)pos.y].noise = 0.15f;

                if (bold && map[(int)pos.x, (int)pos.y].noise < 0.75f)
                {
                    //cells[(int)pos.x+1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x-1, (int)pos.y].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y+1].type.color = Color.blue;
                    //cells[(int)pos.x, (int)pos.y-1].type.color = Color.blue;
                    if (pos.x + 1 < mapGenerator.mapSize)
                        map[(int)pos.x + 1, (int)pos.y].noise = 0.15f;
                    if (pos.x - 1 >= 0)
                        map[(int)pos.x - 1, (int)pos.y].noise = 0.15f;
                    if (pos.y + 1 < mapGenerator.mapSize)
                        map[(int)pos.x, (int)pos.y + 1].noise = 0.15f;
                    if (pos.y - 1 >= 0)
                        map[(int)pos.x, (int)pos.y - 1].noise = 0.15f;
                }
            }
        }
    }
}
