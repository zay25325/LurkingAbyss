using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnEnable()
    {
        var rootElement = GetComponent<UIDocument>().rootVisualElement;
        var startButton = rootElement.Q<Button>("PlayBtn"); // make sure name is the same in UI Builder

        startButton.clicked += () =>
        {
            Debug.Log("Start Button Clicked");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        };
    
    }
}
