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
    /// Grid para este objecto, en este se mirara la colocacion de este objecto en funcion delradius y cellSize
    /// </summary>
    private Vector2[,] personalGrid;

    private readonly Rect rect;
    /// <summary>
    /// Cantidad de objectos que queremos generar
    /// </summary>
    private int amount;
    /// <summary>
    /// radio al cuadrado para generar los objecto con distancioa de separacion de maximo radius^2
    /// </summary>
    private readonly float radius2;  // radius squared
    /// <summary>
    /// Tamaño de la "casilla"
    /// </summary>
    private readonly float cellSize;


    /// <summary>
    /// Lista de posiciones para los objectos generados
    /// </summary>
    private List<Vector2> activeSamples = new List<Vector2>();
    /// <summary>
    /// Crear un Patron de Poisson Disc
    /// </summary>
    /// <param name="width"> Anchura del mapa generado</param>
    /// <param name="height">Longitud de mapa generado</param>
    /// <param name="radius">Cada objecto estará a una distancia mínima de `radio` de cualquier otra muestra, y como máximo a 2 * `radio`.</param>
    public PoissonDiscSampler(float width,float height, float radius,int amount)
    {
        this.amount = amount;
        rect = new Rect(0,0,width,height);
        radius2 = radius * radius;
        cellSize = radius / Mathf.Sqrt(2);
        personalGrid = new Vector2[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(width / cellSize)];
    }

    /// <summary>
    /// Devuelve una secuencia de posibles posiciones que respetan la distancia establecida anteriormente
    /// </summary>
    public IEnumerable<Vector2> Samples()
    {
        //yield return AddSample(new Vector2(Random.value * rect.width, Random.value * rect.height));
        while (activeSamples.Count < amount) 
        {
            if (activeSamples.Count <= 0) AddSample(new Vector2(Random.value * rect.width, Random.value * rect.height));

            //Coger una posicion del la lista aleatoria
            int i = (int)Random.value * activeSamples.Count;

            Vector2 current = activeSamples[i];
            bool found = false;
            //Probar a obtener los candidatos deseados que se encuentren entre [radius,radius^2]
            for (int j = 0; j < amount; j++)
            {
                float angle = 2 * Mathf.PI * Random.value;
                //Generar un numero aleatorio que este dentro de los radios establecidos, por asi decirlo es como generar un numero aletorio que este dentro de un anillo
                float r = Mathf.Sqrt(Random.value * 3 * radius2 + radius2);
                //Posible Posicion
                Vector2 candidate = current + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                if ( rect.Contains(candidate) && IsFarEnough(candidate))
                {
                    found = true;
                    yield return AddSample(candidate);
                    break;
                }
            }
            // Si no encontramos un candidato válido tras "amount" intentos, eliminamos esta muestra de la cola de muestras activas
            if (!found)
            {
                activeSamples[i] = activeSamples[activeSamples.Count - 1];
                activeSamples.RemoveAt(activeSamples.Count - 1);
               
            }
        }
    }

    /// <summary>
    /// Añade la posicion del objecto a la cola de posiciones activas
    /// </summary>
    Vector2 AddSample(Vector2 position)
    {
        activeSamples.Add(position);
        var gridPosition = vector2ToGrid(position);
        personalGrid[gridPosition.x, gridPosition.y] = position;
        return position;
    }

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
    bool IsFarEnough(Vector2 position)
    {
        var gridPos = vector2ToGrid(position);

        int xmin = Mathf.Max(0, gridPos.x - 2);
        int ymin = Mathf.Max(0, gridPos.y - 2);
        int xmax = Mathf.Min(gridPos.x + 2, personalGrid.GetLength(0) - 1);
        int ymax = Mathf.Min(gridPos.y + 2, personalGrid.GetLength(1) - 1);

        for (int y = ymin; y <= ymax; y++)
        {
            for (int x = xmin; x <= xmax; x++)
            {
                // Utilizamos el vector cero para denotar una celda sin rellenar en la cuadrícula
                if (personalGrid[x, y] != Vector2.zero)
                {
                    var d = personalGrid[x, y] - position;
                    return d.x * d.x + d.y * d.y < radius2;
                }
            }
        }
        return true;
        //Nota: Si tomamos (0,0) como muestra, esta sera ignorada
    }
}
