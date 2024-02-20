using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PerlinWorm{

    Vector2 currentDirection; //Direccion aleatoria inicial de nuestro Worm
    Vector2 currentPosition; //Posicion inicial que hayamos elegido del mapa para empezar a crear el rio
    Vector2 convergancePoint; //Punto de convergencia en el "Agua" 
    NoiseSettings noiseSettings; //Misma configuracion que el mapGenerator de tema de octavas, lacuynarity,...
    public bool moveToConvergancepoint = false; //Si queremos usar el punto de convergencia o No
    [Range(0.5f, 0.9f)]
    public float weight = 0.6f;

    public PerlinWorm(NoiseSettings noiseSettings,Vector2 startPosition, Vector2 convergancePoint) 
    {
        this.noiseSettings = noiseSettings;
        this.convergancePoint = convergancePoint;
        currentPosition = startPosition;
        currentDirection = Random.insideUnitCircle.normalized; // genera un vector aleatorio en el plano XY 
        moveToConvergancepoint =true;
    }

    public PerlinWorm(NoiseSettings noiseSettings, Vector2 startPosition)
    {
        this.noiseSettings = noiseSettings;
        currentPosition = startPosition;
        currentDirection = Random.insideUnitCircle.normalized; //genera un vector aleatorio en el plano XY
        moveToConvergancepoint = false;
    }

    private Vector3 GetPerlinNoiseDirection()
    {
        float noiseCell = 0.3f;
        float degrees = 0;
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


}
