using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Procedural/Foliage")]
public class Foliage : ScriptableObject
{
    public string name;

    // DENSITY FEATURES
    public AnimationCurve densityCurve;

    public float density = 0.1f;

    public float noiseScale = 0.1f;

    public int unitSpace = 0;

    // PREFAB PROPERTIES
    public GameObject prefab;
    public int priorityLayer;

    // TRANSFORM
    public bool randomRotation;
    public Vector3 rotation;
    public Vector3 maxRotation;

    public bool randomScale;
    public Vector3 scale;
    public Vector3 maxScale;

    // HEIGHT
    public bool useRandomHeight;
    public Vector2 minMaxHeight;
}


#if UNITY_EDITOR
[CustomEditor(typeof(Foliage))]
class FoliageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var thisEditor = (Foliage)target;
        if (thisEditor == null)
            return;

        // Nombre
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));

        CreateHeader("PREFAB PROPERTIES");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), new GUIContent("prefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("priorityLayer"), new GUIContent("priorityLayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("unitSpace"), new GUIContent("Unit Space Separation"));

        CreateHeader("DENSITY FEATURES");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("densityCurve"), new GUIContent("Density Curve"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("density"), new GUIContent("Density"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("noiseScale"), new GUIContent("Noise Scale"));

        CreateHeader("TRANSFORM");


        // ROTACION
        // Elegir si se quiere rotacion aleatoria entre 2 puntos o constante
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomRotation"), new GUIContent("Random Rotation"));
        if (!thisEditor.randomRotation)
            // Rotacion constante
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotation"), new GUIContent("Rotation"));
        else
        {
            // Rotacion aleatoria entre 2 puntos
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotation"), new GUIContent("MIN rotation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRotation"), new GUIContent("MAX rotation"));
        }

        EditorGUILayout.Space();

        // ESCALA
        // Elegir si se quiere escale aleatoria entre 2 puntos o constante
        EditorGUILayout.PropertyField(serializedObject.FindProperty("randomScale"), new GUIContent("Random Scale"));
        if (!thisEditor.randomScale)
            // Rotacion constante
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"), new GUIContent("Scale"));
        else
        {
            // Rotacion aleatoria entre 2 puntos
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"), new GUIContent("MIN Scale"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxScale"), new GUIContent("MAX Scale"));
        }



        CreateHeader("HEIGHT");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("useRandomHeight"), new GUIContent("Use Random Height"));

        if (thisEditor.useRandomHeight)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minMaxHeight"), new GUIContent("Min / Max Height"));

        serializedObject.ApplyModifiedProperties();

        //DrawDefaultInspector();
    }


    void CreateHeader(string headerText)
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
    }

}
#endif