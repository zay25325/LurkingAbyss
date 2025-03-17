using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelTransitionManager : MonoBehaviour
{
    static LevelTransitionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Week9Demo");
    }

}


[CustomEditor(typeof(LevelTransitionManager))]
public class LevelTransitionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelTransitionManager levelTransitionManager = (LevelTransitionManager)target;
        if (GUILayout.Button("Build Layout"))
        {
            levelTransitionManager.NextLevel();
        }

    }
}