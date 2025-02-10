using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DevTriggerNoiseEvent : MonoBehaviour
{
	[SerializeField] float volume = 5f;
	public void CreateNoise()
    {
		NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, volume, new List<EntityInfo.EntityTags>());
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DevTriggerNoiseEvent))]
public class DevTriggerNoiseEventEditor : Editor // I seem to be making some very long names
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Create Noise"))
		{
			DevTriggerNoiseEvent noiseTrigger = (DevTriggerNoiseEvent)target;
			noiseTrigger.CreateNoise();
		}
	}
}
#endif
