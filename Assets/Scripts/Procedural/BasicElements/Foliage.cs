using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Foliage")]
public class Foliage : ScriptableObject
{

    [Serializable]
    public enum TypeOfFoliage { texture=0, prefab=1 };
    [Serializable]
    enum FoliageColorBehaviour { Random, Height, Scale }

    [SerializeField]
    public string Name;


    [Header("Type Of Foliage")]
    public TypeOfFoliage typeOfFoliage = TypeOfFoliage.prefab;

    [ShowWhen("typeOfFoliage", TypeOfFoliage.texture)]
    public Sprite sprite;

    [ShowWhen("typeOfFoliage", TypeOfFoliage.prefab)]
    public GameObject prefab;


    [Space(1),Header("Density features")]

    // Curva de densidad basada en la altura del terreno
    public AnimationCurve densityCurve;
   
    public float Density = 0.1f;

    public float NoiseScale = 0.1f;

    [Header("Transform")]
    [ShowWhen("typeOfFoliage", TypeOfFoliage.prefab)]
    [SerializeField]
    Vector3 rotation;

    [ShowWhen("typeOfFoliage", TypeOfFoliage.prefab)]
    [SerializeField]
    Vector3 scale;

    [Header("Height")]
    [SerializeField]
    bool useRandomHeight;

    [ShowWhen("useRandomHeight")]
    [SerializeField]
    float minHeight;

    [ShowWhen("useRandomHeight")]
    [SerializeField]
    float maxHeight;



    // Solo texture
    [ShowWhen("typeOfFoliage", TypeOfFoliage.texture)]
    [Header("Color")]
    [SerializeField]
    bool useColor;

   

    [HideIfEnumValue("typeOfFoliage", HideIf.Equal, (int)TypeOfFoliage.prefab)]
    [HideIf("useColor", false)]
    [SerializeField]
    FoliageColorBehaviour behaviour;

    [HideIfEnumValue("typeOfFoliage", HideIf.Equal, (int)TypeOfFoliage.prefab)]
    [HideIf("useColor", false)]
    [SerializeField]
    Gradient gradient;

}
