using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    void OnEnable()
    {
        var rootElement = GetComponent<UIDocument>().rootVisualElement;

        // make sure name is the same in UI Builder
        var StartButton = rootElement.Q<Button>("PlayBtn");
        var LogsButton = rootElement.Q<Button>("LogsBtn");
        var SettingsButton = rootElement.Q<Button>("SettingsBtn");
        var QuitButton = rootElement.Q<Button>("QuitBtn");

        StartButton.clicked += () =>
        {
            Debug.Log("Start Button Clicked");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        };

        LogsButton.clicked += () =>
        {
            Debug.Log("Logs Button Clicked");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("LogsScene");
        };

        SettingsButton.clicked += () => 
        {
            Debug.Log("Settings Button Clicked");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SettingsScene");
        };

        QuitButton.clicked += () => 
        {
            Debug.Log("Quit Button Clicked");
            // not sure how to quit :p

        };

    }
}
