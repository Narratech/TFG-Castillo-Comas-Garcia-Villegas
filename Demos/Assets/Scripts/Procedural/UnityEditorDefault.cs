using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class UnityEditorDefault : MonoBehaviour
{
    //[HideInInspector]
    public string title = "dsfegr";
    //[HideInInspector]
    public int age = 22;

    public bool showAge;
}


#if UNITY_EDITOR
[CustomEditor(typeof(UnityEditorDefault))]
class DefaultEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        var thing = (UnityEditorDefault)target;

        if (thing == null)
            return;

        Undo.RecordObject(thing, "Change this shit");

        if (GUILayout.Button("Increment"))
            thing.age++;

        if (thing.showAge)
            GUILayout.Label("Age = " + thing.age);

        EditorGUI.BeginChangeCheck();
        thing.title = EditorGUILayout.TextField(thing.title);
        if (EditorGUI.EndChangeCheck())
            Debug.Log("NEW THING = " + thing.title);

        DrawDefaultInspector();
    }

}
#endif
