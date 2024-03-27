using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Algoritmo Poisson Disc sampling usando Bridson,s algorithm 
/// Adapted from Gregory Schlomoff's: http://gregschlom.com/devlog/2014/06/29/Poisson-disc-sampling-Unity.html
/// </summary>
public class PoissonDiscSampler 
{
    /// <summary>
    /// Grid para este objeto, en este se mirara la colocacion de este objeto en funcion del radius_ y cellSize
    /// </summary>
    private int [,] grid;

    private readonly Rect rect;
    /// <summary>
    /// Cantidad de objectos que queremos generar
    /// </summary>
    private int amount;
    /// <summary>
    /// radio al cuadrado para generar los objecto con distancioa de separacion de maximo radius_^2
    /// </summary>
    private readonly float radius2;  // radius_ squared
    private float radius;
    /// <summary>
    /// Tamaño de la "casilla"
    /// </summary>
    private readonly float cellSize;


    /// <summary>
    /// Lista de posiciones para los objectos generados
    /// </summary>
    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> spawnPoints = new List<Vector2>();
    /// <summary>
    /// Crear un Patron de Poisson Disc
    /// </summary>
    /// <param name="width"> Anchura del mapa generado</param>
    /// <param name="height"> Longitud de mapa generado</param>
    /// <param name="radius_"> Cada objeto estará a una distancia mínima de `radio` de cualquier otra muestra, y como máximo a 2 * `radio`.</param>
    public PoissonDiscSampler(float width,float height, float radius_,int amount)
    {
        this.amount = amount;
        rect = new Rect(0,0,width,height);
        radius = radius_;
        radius2 = radius_ * radius_;
        cellSize = radius_ / Mathf.Sqrt(2);
        grid = new int[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];

        spawnPoints.Add(new Vector2(width/2, height/2));
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
    /// Añade la posicion del objecto a la cola de posiciones activas
    /// </summary>
    //Vector2 AddSample(Vector2 position)
    //{
    //    //if (position.y < 0) position.y *= -1;
    //    points.Add(position);
    //    var gridPosition = vector2ToGrid(position);
    //    grid[gridPosition.x, gridPosition.y] = position;
    //    return position;
    //}

    /// <summary>
    /// Conversion de la posicion del objecto para poder guardarlo en el grid
    /// </summary>
    Vector2Int vector2ToGrid(Vector2 position)
    {
        return new Vector2Int((int)(position.x/cellSize), (int)(position.y/cellSize));
    }


    /// <summary>
    /// Comprobar la disponibilidad y proximidad de la posicion candidata
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    //bool IsFarEnough(Vector2 position)
    //{
    //    var gridPos = vector2ToGrid(position);

    //    int xmin = Mathf.Max(0, gridPos.x - 2);
    //    int ymin = Mathf.Max(0, gridPos.y - 2);
    //    int xmax = Mathf.Min(gridPos.x + 2, grid.GetLength(0) - 1);
    //    int ymax = Mathf.Min(gridPos.y + 2, grid.GetLength(1) - 1);

    //    for (int y = ymin; y <= ymax; y++)
    //    {
    //        for (int x = xmin; x <= xmax; x++)
    //        {
    //            // Utilizamos el vector cero para denotar una celda sin rellenar en la cuadrícula
    //            if (grid[x, y] != Vector2.zero)
    //            {
    //                var d = grid[x, y] - position;
    //                return d.x * d.x + d.y * d.y < radius2;
    //            }
    //        }
    //    }
    //    return true;
    //    //Nota: Si tomamos (0,0) como muestra, esta sera ignorada
    //}

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
