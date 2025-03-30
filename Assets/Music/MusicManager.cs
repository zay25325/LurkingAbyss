using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    AudioSource audioSource;
    MusicState currentState = MusicState.None;

    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip tutorialClip;
    [SerializeField] List<AudioClip> levelClips;
    [SerializeField] AudioClip escapeClip;
    [SerializeField] AudioClip deathClip;


    public enum MusicState 
    {
        MainMenu,
        Tutorial,
        Level,
        Escape,
        Death,

        None
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = GetStateAudio(currentState);
            audioSource.Play();
        }
    }

    /*
    Method: SwitchState
    Description: Only changes the audio track if the state if different
    Parameters: 
        MusicState newState : state we are switching to
    Returns: void
    */
    public void SwitchState(MusicState newState)
    {
        if (newState != currentState)
        {
            currentState = newState;
            audioSource.Stop();
            audioSource.clip = GetStateAudio(currentState);
            audioSource.Play();
        }
    }

    private AudioClip GetStateAudio(MusicState state)
    {
        AudioClip audio = null;
        switch (state)
        {
            case MusicState.MainMenu:
                audio = mainMenuClip;
                break;
            case MusicState.Tutorial:
                audio = tutorialClip;
                break;
            case MusicState.Level:
                audio = levelClips[Random.Range(0, levelClips.Count)];
                break;
            case MusicState.Escape:
                audio = escapeClip;
                break;
            case MusicState.Death:
                audio = deathClip;
                break;
        }
        return audio;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                SwitchState(MusicState.MainMenu);
                break;
            case "IntroLevel":
                SwitchState(MusicState.Tutorial);
                break;
            case "Week9Demo":
                SwitchState(MusicState.Level);
                break;
            case "EndingScene":
                SwitchState(MusicState.Escape);
                break;
        }
    }
}
