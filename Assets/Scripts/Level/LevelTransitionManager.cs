using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelTransitionManager : MonoBehaviour
{
    public static LevelTransitionManager Instance;

    public int LevelNumber { get; set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void NextLevel()
    {
        // Trigger level transition animation later.
        LevelNumber++;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Week9Demo");
    }

    public void StartEndingDelay()
    {
        StartCoroutine(EndingAnimation());
    }

    private IEnumerator EndingAnimation()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("EndingScene");
    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(LevelTransitionManager))]
public class LevelTransitionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelTransitionManager levelTransitionManager = (LevelTransitionManager)target;
        if (GUILayout.Button("Next Level"))
        {
            levelTransitionManager.NextLevel();
        }

    }
}

#endif