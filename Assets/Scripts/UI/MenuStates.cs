using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStates : MonoBehaviour
{

    public GameObject pauseMenu; // pause menu canvas refernce
    public GameObject deathMenu; // death menu canvas reference

    public Button resumeBtn, deathResumeBtn, quitBtn;

    public bool isPaused;

    void Start()
    {
        // set both menus off
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);

        // event listeners in the pause menu
        resumeBtn.onClick.AddListener(ResumeGame);
        quitBtn.onClick.AddListener(SendToMainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ShowDeathMenu()
    {
        deathMenu.SetActive(true);

    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // stops all animations and what not
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SendToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
