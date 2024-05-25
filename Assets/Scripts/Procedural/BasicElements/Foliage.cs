using UnityEngine;
using UnityEditor;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(menuName = "Procedural/Foliage")]
public class Foliage : ScriptableObject
{
    // DENSITY FEATURES
    [Tooltip("Densidad de los objetos dependiendo de la altura. Si es una curva ascendente, habrá más objetos a mayor altura")]
    public AnimationCurve densityCurve;
    [Tooltip("Densidad de las instancias. Cuanto mayor sea, mayor numero de instancias apareceran")]
    public float density = 0.1f;
    [Tooltip("Los objetos se generan mediante ruido. Este valor indica la escala de este")]
    public float noiseScale = 0.1f;
    [Tooltip("Unidades de separacion de este objeto con otros")]
    public int unitSpace = 0;

    // PREFAB PROPERTIES
    [Tooltip("Prefab asignado a este objeto")]
    public GameObject prefab;
    [Tooltip("Indica si el objeto tiene distanciacion con otros")]
    public bool requireDistance = false; //Esta variable almacena los objetos que pueden instanciarse CON/SIN necesidad de tener distancia de separación entre ellos.

    // TRANSFORM
    [Tooltip("Si es True la rotacion de las instancias puede variar entre un minimo y un maximo")]
    public bool randomRotation;
    public Vector3 rotation;
    public Vector3 maxRotation;
   
    [Tooltip("Si es True la escala de las instancias puede variar entre un minimo y un maximo")]
    public bool randomScale;
    public Vector3 scale;
    public Vector3 maxScale;


    //ADVANCED SETTINGS
    [Tooltip("Adaptacion a las inclinaciones del terreno")]
    public bool environment_rotation; //rotacion con el enviroment cuestas y cosas asi
    [Tooltip("Porcentaje del objeto que se puede hundir en el suelo")]
    public float subsidence_in_the_ground = 0f; // % del objeto que se puede hundir en el suelo
}


#if UNITY_EDITOR
[CustomEditor(typeof(Foliage))]
class FoliageEditor : Editor
{
    private readonly string[] opciones = new string[] { "No", "Yes" };
    public override void OnInspectorGUI()
    {

        var thisEditor = (Foliage)target;
        if (thisEditor == null)
            return;

        CreateHeader("PREFAB PROPERTIES");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("prefab"), new GUIContent("prefab"));

        CreateHeader("Distance Separation");
        thisEditor.requireDistance = GUILayout.SelectionGrid(thisEditor.requireDistance ? 1 : 0, opciones, 2) == 1;

        if (thisEditor.requireDistance)
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

        CreateHeader("Advanced Settings");

        EditorGUILayout.PropertyField(serializedObject.FindProperty("environment_rotation"), new GUIContent("Environment Rotation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("subsidence_in_the_ground"), new GUIContent("Subsidence in the ground"));

        serializedObject.ApplyModifiedProperties();
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