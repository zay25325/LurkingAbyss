using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RaycastTester : MonoBehaviour
{
    [SerializeField] public Vector2 raycastDestination;
    [SerializeField] public float raycastDistance;
}

#if UNITY_EDITOR
[CustomEditor(typeof(RaycastTester))]
public class RaycastTesterEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("raycast"))
		{
			var raycastTester = (RaycastTester)target;
			Vector2 direction = raycastTester.raycastDestination - (Vector2)raycastTester.transform.position;

			Debug.DrawRay((Vector2)raycastTester.transform.position, direction);
			RaycastHit2D hit = Physics2D.Raycast((Vector2)raycastTester.transform.position, direction, raycastTester.raycastDistance);

			if (hit.collider == null)
            {
				Debug.Log("Raycast did not hit any targets");
            }
			else
            {
				Debug.Log($"Raycast hit at: {hit.point}");
				Debug.Log($"Raycast normals: {hit.normal}");
			}
		}
	}

}
#endif
