using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{

	public Color[] baseColours;
	[Range(0, 1)]
	public float[] baseStartHeights;

	[Range(0, 1)]
	public float[] baseBlends;


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
		material.SetInt("baseColourCount", baseColours.Length);
		material.SetColorArray("baseColours", baseColours);
		material.SetFloatArray("baseStartHeights", baseStartHeights);
		material.SetFloatArray("baseBlends", baseBlends);


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

}