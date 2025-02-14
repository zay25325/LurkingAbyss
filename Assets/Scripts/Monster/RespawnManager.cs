/*
File: RespawnManager.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/14/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Name: RespawnManager
Purpose: The entire game object for monsters should be disabled after they die so we do not need to track all of the many components to disable.
  The problem is that Unity does not let you manipulate game objects unless you are on the main thread. Similarly, any timer on the monster would be disabled with the game object.
  Due to this we need a seperate game object to run the timer to re-enabled the monster's game object.
*/
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
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

    struct RespawnTimer
    {
        public MonsterController Controller;
        public float RespawnTime;

        public RespawnTimer(MonsterController controller, float respawnTime)
        {
            Controller = controller;
            RespawnTime = respawnTime;
        }
    }

    private List<RespawnTimer> respawnTimers = new List<RespawnTimer>();
    float timeSinceStart = 0;
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        for (int i = respawnTimers.Count-1; i >= 0; i--)
        {
            if (respawnTimers[i].RespawnTime < timeSinceStart)
            {
                if (respawnTimers[i].Controller != null)
                {
                    respawnTimers[i].Controller.Respawn();
                }
                respawnTimers.RemoveAt(i);
            }
        }
    }

    public void StartRespawnTimer(MonsterController controller, float timeUntilRespawn)
    {
        respawnTimers.Add(new RespawnTimer(controller, timeSinceStart + timeUntilRespawn));
    }
}
