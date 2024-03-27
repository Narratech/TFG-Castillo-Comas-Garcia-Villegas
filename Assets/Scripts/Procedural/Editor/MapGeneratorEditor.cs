using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if (mapGen == null)
            return;

        //Si cualquier elemento del editor de MapGenerator se altera
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
            mapGen.mapSize = (int)Mathf.Floor(mapGen.mapSize / 10.0f) * 10; //Tamaño del mapa multiplo de 10
            if (mapGen.drawMode == MapGenerator.DrawMode.Cartoon)
            {
                mapGen.sizePerBlock = 1f;
            }
            if (mapGen.sizePerBlock < 1f) mapGen.sizePerBlock = 1f;
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
#endif