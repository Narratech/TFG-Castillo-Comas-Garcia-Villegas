using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Permite pintar el mapa generado 
/// </summary>
public class MapDisplay : MonoBehaviour{
    public Renderer textureRender;
    public void DrawTextureMap(Texture2D texture){
        textureRender.sharedMaterial.mainTexture= texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
    public void ActiveMap(bool act){
        textureRender.gameObject.SetActive(act);
    }
}
