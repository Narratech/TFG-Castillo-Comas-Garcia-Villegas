using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUpdater : MonoBehaviour
{

    [SerializeField]
    TextureData textureData;

    [SerializeField]
    Material terrainMaterial;


    // Se ejecuta cada vez que se modifica algo en el inspector de este script
    private void OnValidate()
    {
        // Suscribir un metodo dentro del propio textureData al evento de OnValuesUpdated del TextureData
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }

    // Se ejecuta cada vez que se modifica algo en en el ScriptableObject del textureData
    void OnTextureValuesUpdated()
    { 
        textureData.ApplyToMaterial(terrainMaterial);
    }
}
