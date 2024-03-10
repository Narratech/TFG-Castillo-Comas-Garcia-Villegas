using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        
        //Si cualquier elemento del editor de MapGenerator se altera
        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate){
                
               mapGen.GenerateMap();
            }
            mapGen.mapSize =(int)Mathf.Floor(mapGen.mapSize / 10.0f) * 10; //Tamaño del mapa multiplo de 10
        }
        if (GUILayout.Button("Generate")){
            mapGen.GenerateMap();
        }
    }
}
