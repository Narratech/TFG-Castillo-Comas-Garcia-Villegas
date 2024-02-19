using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject
{

	public event System.Action OnValuesUpdated;
	public bool autoUpdate;


	protected virtual void OnValidate()
	{
		if (autoUpdate)
		{
			// Ejecuta el metodo NotifyOfUpdatedValues despues de que compilen los shaders,
			// para aplicar los valores
			UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
		}
	}

	public void NotifyOfUpdatedValues()
	{
		UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
		if (OnValuesUpdated != null)
		{
			OnValuesUpdated();
		}
	}

}