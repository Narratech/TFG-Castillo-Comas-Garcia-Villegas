using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Procedural Map Creator", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Cabeza Hovos", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();

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