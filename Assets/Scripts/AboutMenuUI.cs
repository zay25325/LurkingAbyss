using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AboutMenu : MonoBehaviour
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

        var BackButton = rootElement.Q<Button>("BackBtn");
        BackButton.clicked += () =>
        {
            //Debug.Log("Back Button Clicked");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        };
        //AboutMenuButtons(BackButton);
    }

    //private void AboutMenuButtons(Button BackButton)
    //{
    //    BackButton.clicked += () =>
    //    {
    //        Debug.Log("Back Button Clicked");
    //        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    //    };
    //}
}
