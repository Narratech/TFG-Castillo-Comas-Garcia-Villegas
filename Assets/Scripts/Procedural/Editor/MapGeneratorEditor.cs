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

        EditorGUILayout.Space();
        GUILayout.Label("Procedural Map Creator", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16 });
        GUILayout.Label("By Cabeza Hovos", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        GUILayout.Label("version 1.0.1", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, fontSize = 12 });
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        ////
        EditorGUILayout.Space();
        GUILayout.Label("Basics Elements", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
        ////

        EditorGUI.BeginChangeCheck();//Para poder ver si se han producido cambios en el editor


        EditorGUILayout.PropertyField(serializedObject.FindProperty("drawMode"), new GUIContent("Draw Mode"));

        //GameObjects
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gameObjectMap3D"), new GUIContent("GameObject Map3D"));
        
        //Basic Elements
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mapSize"), new GUIContent("Map Size"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"), new GUIContent("Seed"));

        if (mapGen.drawMode == MapGenerator.DrawMode.CubicMap)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sizePerBlock"), new GUIContent("Size Per Block"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"), new GUIContent("Offset"));

        ////
        EditorGUILayout.Space();
        GUILayout.Label("Biomes", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
        ////
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("biomeGenerator"), new GUIContent("Biome Generator"));

        ////
        EditorGUILayout.Space();
        GUILayout.Label("Curves", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
        ////

        //Curves
        EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseTransition"), new GUIContent("Noise Transition"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("heightTransition"), new GUIContent("Height Transition"));

        ////
        EditorGUILayout.Space();
        GUILayout.Label("InterestPoints", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
        ////

        //InterestPoints 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interestPoints"), new GUIContent("Interest Points"));

        ////
        EditorGUILayout.Space();
        GUILayout.Label("Boolean Options", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold, fontSize = 14 });
        ////

        //Boleanos
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isIsland"), new GUIContent("Is Island"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdate"), new GUIContent("Auto Update"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoRegenerate"), new GUIContent("Auto Regenerate"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("generateObjects"), new GUIContent("Generate Objects"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("generateInterestPoints"), new GUIContent("Generate Interest Points"));

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }

        serializedObject.ApplyModifiedProperties();



        //Si cualquier elemento del editor de MapGenerator se altera
        if (EditorGUI.EndChangeCheck())
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
    }
}
#endif