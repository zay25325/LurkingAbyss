using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugMonsterState : MonsterState
{
    // While not necessary in this state to define the MonoBehaviour events. I want to do this as a best practice since this script will likely get referenced in making new ones

    void Start()
    {
        // nothing to do on Start
    }

    void Update()
    {
        // nothing to do on Update
    }

    private void OnEnable()
    {
        // nothing to do when enabled
    }

    private void OnDisable()
    {
        // nothing to do when disabled
    }

    public override void OnNoiseDetection(Vector2 pos, float volume)
    {
        Debug.Log($"Noise Detected at: {pos} with a total volume at {volume}");
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        Debug.Log($"Entity seen: {collider.gameObject}");
    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {
        Debug.Log($"Entity left sight: {collider.gameObject}");
    }

    public override void OnTouchEnter(Collision2D collision)
    {
        Debug.Log($"Entity touched: {collision.gameObject}");
    }

    public override void OnTouchExit(Collision2D collision)
    {
        Debug.Log($"Entity stopped touching: {collision.gameObject}");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DebugMonsterState))]
public class DebugMonsterStateEditor : Editor // I seem to be making some very long names
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Test State Switching"))
        {
            DebugMonsterState debugState = (DebugMonsterState)target;
            debugState.controller.SwitchState<DebugSwitchMonsterState>();
        }
    }
}
#endif