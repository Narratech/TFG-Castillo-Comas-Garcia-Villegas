using UnityEngine;
using System.Collections;
using System.Linq;


[CreateAssetMenu()]
public class TextureData : UpdatableData
{
	const int textureSize = 512;
	const TextureFormat textureFormat = TextureFormat.RGB565;




	//public Color[] baseColours;
	//[Range(0, 1)]
	//public float[] baseStartHeights;

	//[Range(0, 1)]
	//public float[] baseBlends;

	public Layer[] layers;



	[Range(0, 1)]
	public float middlePosition;
	[Range(0, 1)]
	public float blendEffect;


	float savedMinHeight;
	float savedMaxHeight;

	// Se llama cada vez que se actualiza cualquier valor de este scriptableObject
	public void ApplyToMaterial(Material material)
	{
		Debug.Log("ApplyToMaterial");

		// Tienen que tener exactamente los nombres de las variables dentro del shader
		material.SetInt("layerCount", layers.Length);
		material.SetColorArray("baseColours", layers.Select(x => x.tint).ToArray());
		material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
		material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrenght).ToArray());
		material.SetFloatArray("baseColourStrenght", layers.Select(x => x.tintStrenght).ToArray());
		material.SetFloatArray("baseTexturesScales", layers.Select(x => x.textureScale).ToArray());
		Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
		material.SetTexture("baseTextures", texturesArray);


		material.SetFloat("middlePosition", middlePosition);
		material.SetFloat("blendEffect", blendEffect);


		UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
	}

	public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
	{
		savedMinHeight = minHeight;
		savedMaxHeight = maxHeight;

		material.SetFloat("minHeight", minHeight);
		material.SetFloat("maxHeight", maxHeight);
	}


	Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
		Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);

        for (int i = 0; i < textures.Length; i++)
			textureArray.SetPixels(textures[i].GetPixels(), i);

		return textureArray;
	}


    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrenght;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrenght;
        public float textureScale;
    }
}