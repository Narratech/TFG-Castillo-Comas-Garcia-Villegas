using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Algoritmo Poisson Disc sampling 
/// Adapted from Sebastian Lague: https://www.youtube.com/watch?v=7WcmyxyFO7o
/// </summary>
public class PoissonDiscSampler 
{
    /// <summary>
    /// Grid para este objeto, en este se mirara la colocacion de este objeto en funcion del radius_ y cellSize
    /// </summary>
    private int [,] grid;

    private readonly Rect rect;
    /// <summary>
    /// Cantidad de objetos que queremos generar
    /// </summary>
    private int amount;
    /// <summary>
    /// radio  generar los objeto con distancia de separacion de maximo radius_
    /// </summary>
    private float radius;
    /// <summary>
    /// Tamaño de la "casilla"
    /// </summary>
    private readonly float cellSize;


    /// <summary>
    /// Lista de posiciones para los objetos generados
    /// </summary>
    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> spawnPoints = new List<Vector2>();
    /// <summary>
    /// Crear un Patron de Poisson Disc
    /// </summary>
    /// <param name="size"> Anchura del mapa generado</param>
    /// <param name="size"> Longitud de mapa generado</param>
    /// <param name="radius_"> Cada objeto estará a una distancia mínima de `radio` de cualquier otra muestra, y como máximo a 2 * `radio`.</param>
    public PoissonDiscSampler(float size, float radius_, int amount)
    {
        this.amount = amount;
        rect = new Rect(0, 0, size, size);
        radius = radius_;
        cellSize = radius_ / Mathf.Sqrt(2);
        grid = new int[Mathf.CeilToInt(size / cellSize), Mathf.CeilToInt(size / cellSize)];

        spawnPoints.Add(new Vector2(size/2, size/2));
    }

    /// <summary>
    /// Devuelve una secuencia de posibles posiciones que respetan la distancia establecida anteriormente
    /// </summary>
    public List<Vector2> Samples()
    {

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < amount; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);
                if (IsValid(candidate) && points.Count < amount)
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }
    /// <summary>
    /// Comprobar la disponibilidad y proximidad de la posicion candidata
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    bool IsValid(Vector2 candidate)
    {
        if (candidate.x >= 0 && candidate.x < rect.width && candidate.y >= 0 && candidate.y < rect.height)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}