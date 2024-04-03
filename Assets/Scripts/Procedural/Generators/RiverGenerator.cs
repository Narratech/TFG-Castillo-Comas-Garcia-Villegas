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

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        if (mapGenerator == null)
            mapGenerator = gameObject.AddComponent<MapGenerator>();
    }
}
