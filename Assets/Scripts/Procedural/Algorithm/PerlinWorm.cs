using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerlinWorm{

    Vector2 currentDirection; //Direccion aleatoria inicial de nuestro Worm
    Vector2 currentPosition; //Posicion inicial que hayamos elegido del mapa para empezar a crear el rio
    Vector2 convergancePoint; //Punto de convergencia en el "Agua" 
    float noiseCell; //Misma configuracion que el mapGenerator de tema de octavas, lacuynarity,...
    public bool moveToConvergancepoint = false; //Si queremos usar el punto de convergencia o No
    [Range(0.5f, 0.9f)]
    public float weight = 0.6f;

    public PerlinWorm(float noiseCell,Vector2 startPosition, Vector2 convergancePoint) 
    {
        this.noiseCell = noiseCell;
        this.convergancePoint = convergancePoint;
        currentPosition = startPosition;
        currentDirection = Random.insideUnitCircle.normalized; // genera un vector aleatorio en el plano XY 
        moveToConvergancepoint =true;
    }

    public PerlinWorm(float noiseCell, Vector2 startPosition)
    {
        this.noiseCell = noiseCell;
        currentPosition = startPosition;
        currentDirection = Random.insideUnitCircle.normalized; //genera un vector aleatorio en el plano XY
        moveToConvergancepoint = false;
    }

    public static float RangeMap(float inputValue, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (inputValue - inMin) * (outMax - outMin) / (inMax - inMin);
    }

    private Vector3 GetPerlinNoiseDirection()
    {
        float degrees = RangeMap(noiseCell, 0, 1, -90, 90);
        return (Quaternion.AngleAxis(degrees, Vector3.forward) * currentDirection).normalized;
    }

    /// <summary>
    /// Ir moviendose hacia el punto de convergencia de forma gradual y constante
    /// </summary>
    /// <returns></returns>
    public Vector2 MoveTowardsConvergancePoint()
    {
        Vector3 direction = GetPerlinNoiseDirection();
        var directionToConvergancePoint = (convergancePoint - currentPosition).normalized;
        var endDirection = ((Vector2)direction * (1 - weight) + directionToConvergancePoint * weight).normalized;
        currentPosition += endDirection;
        return currentPosition;
    }

    public Vector2 Move()
    {
        Vector3 direction = GetPerlinNoiseDirection();
        currentPosition += (Vector2)direction;
        return currentPosition;
    }

    public List<Vector2> MoveLength(int length)
    {
        var list = new List<Vector2>();
        foreach (var item in Enumerable.Range(0,length))
        {
            if (moveToConvergancepoint)
            {
                var result = MoveTowardsConvergancePoint();
                list.Add(result);
                if(Vector2.Distance(convergancePoint,result) < 1)
                {
                    break;
                }
            }
            else
            {
                var result = Move();
                list.Add(result);
            }
        }
        if (moveToConvergancepoint)
        {
            while (Vector2.Distance(convergancePoint, currentPosition) < 1)
            {
                weight = 0.9f;
                var result = MoveTowardsConvergancePoint();
                list.Add(result);
                if (Vector2.Distance(convergancePoint, result) < 1)
                {
                    break;
                }
            }
        }
        return list;
    }
}
