using UnityEngine;
using UnityEditor;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(menuName = "Procedural/Foliage")]
public class Foliage : ScriptableObject
{
    // DENSITY FEATURES
    public AnimationCurve densityCurve;

    public float density = 0.1f;

    public float noiseScale = 0.1f;

    public int unitSpace = 0;

    // PREFAB PROPERTIES
    public GameObject prefab;
    public bool folliage = false; //Esta variable almacena los objetos que pueden instanciarse CON/SIN necesidad de tener distancia de separación entre ellos.

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

    //ADVANCED SETTINGS
    public bool environment_rotation; //rotacion con el enviroment cuestas y cosas asi
    public float subsidence_in_the_ground = 0f; // % del objeto que se puede hundir en el suelo
}


#if UNITY_EDITOR
[CustomEditor(typeof(Foliage))]
class FoliageEditor : Editor
{
    private readonly string[] opciones = new string[] { " Require Distance Separation", "No Require Distance Separation" };
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var thisEditor = (Foliage)target;
        if (thisEditor == null)
            return;

        CreateHeader("PREFAB PROPERTIES");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), new GUIContent("prefab"));

        //EditorGUILayout.PropertyField(serializedObject.FindProperty("folliage"), new GUIContent("Folliage"));
     
        thisEditor.folliage = GUILayout.SelectionGrid(thisEditor.folliage ? 1 : 0, opciones, 2) == 1;

        if (!thisEditor.folliage)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unitSpace"), new GUIContent("Unit Space Separation"));
        }

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

        CreateHeader("Advanced Settings");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("environment_rotation"), new GUIContent("Environment Rotation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("subsidence_in_the_ground"), new GUIContent("Subsidence in the ground"));
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