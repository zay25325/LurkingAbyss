using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        var StartButton = rootElement.Q<Button>("PlayBtn");
        var LogsButton = rootElement.Q<Button>("LogsBtn");
        var SettingsButton = rootElement.Q<Button>("SettingsBtn");
        var QuitButton = rootElement.Q<Button>("QuitBtn");
        var AboutButton = rootElement.Q<Button>("AboutBtn");
        Thread thread = new Thread(() => MainMenuButtons(StartButton, LogsButton, SettingsButton, AboutButton, QuitButton));
        thread.Start();
        thread.Join();
        //MainMenuButtons(StartButton, LogsButton, SettingsButton, AboutButton, QuitButton);
    }

    private void MainMenuButtons(Button StartButton, Button LogsButton, Button SettingsButton, Button AboutButton, Button QuitButton)
    {
        StartButton.clicked += () =>
        {
            Debug.Log("Start Button Clicked");
            UnityEngine.SceneManagement.SceneManager.LoadScene("IntroLevel");
        };

        QuitButton.clicked += () =>
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        };
    }
}
