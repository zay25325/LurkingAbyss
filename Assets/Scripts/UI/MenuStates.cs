using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuStates : MonoBehaviour
{

    public GameObject pauseMenu; // pause menu canvas refernce
    public GameObject deathMenu; // death menu canvas reference

    public Button resumeBtn, resumeQuitBtn, deathResumeBtn, deathQuitBtn;

    public bool isPaused;
    public bool isDead;

    void Start()
    {
        // set both menus off
        pauseMenu.SetActive(false);
        deathMenu.SetActive(false);

        // event listeners in the pause menu
        resumeBtn.onClick.AddListener(ResumeGame);
        resumeQuitBtn.onClick.AddListener(SendToMainMenu);

        // death menu buttons
        deathResumeBtn.onClick.AddListener(ResumeAfterDeath);
        deathQuitBtn.onClick.AddListener(SendToMainMenu);
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
        // add other stuff here
        Time.timeScale = 0f; // stops all animations and what not
        isDead = true;
    }

    private void ResumeAfterDeath()
    {
        deathMenu.SetActive(false);
        isDead = false;
        Time.timeScale = 1f;
        // somehow restart game
        LevelTransitionManager.Instance.NextLevel();
        Debug.Log("restarted");
    }

    private void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // stops all animations and what not
        isPaused = true;
    }

    private void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void SendToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
