using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public enum ItemSlots
{
    Primary,
    Secondary,
    Melee,
    Throwable,
    Utility
}

//private struct PlayerSettings
public struct PlayerSettings
{
    float mouseSensitivity;
    enum ItemSlots
    {
        Primary,
        Secondary,
        Melee,
        Throwable,
        Utility
    }
}

//private struct GameSettings
public struct GameSettings
{
    float masterVolume;
}

public class SettingsMenuUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /* In order to save Player and Game settings, we could use a 
         * JSON or binary file since this isnt going to be a multiplayer game
         */

        /* 
         * first open the JSON/binary file and read all the data into a hashtable (or any structure)
         * then we can use the contents read from the table and apply it to the structs
         * above
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        var rootElement = GetComponent<UIDocument>().rootVisualElement;

        var backButton = rootElement.Q<Button>("BackBtn");
        backButton.clicked += () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        };
    }

    private void BackButton_clicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
