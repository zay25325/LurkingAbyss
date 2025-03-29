/*
Class Name: PlayerStats
Description: This class is a component of the player game object and represents the player's health and shields. 
             It contains the properties and methods specific to the player's health and shields.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float health = 1f;
    [SerializeField] private float shields = 4f;

    private float maxHealth = 1f;
    private float maxShields = 4f;

    [SerializeField] 
    private float playerSpeed = 5f;    // Speed of the player

    [SerializeField] 
    private float sneakSpeed = 2f;    // Speed of the player when sneaking

    [SerializeField]
    private float dashSpeed = 3f;    // Speed of the player rotation

    //private variables
    [SerializeField]
    private float originalSpeed = 0f;    // Original speed of the player

    [SerializeField]
    private int ichorSamples = 0;    // Number of ichor samples the player has

    [SerializeField]
    private int teleporterShardCount = 0;    // Number of teleportation shards the player has

    private int playerNoise = 0;    // Noise level of the player
    private OnHitEvents hitEvents;
    private PlayerController playerController;


    public float Health 
    { 
        get => health; 
        set => health = Mathf.Min(value, maxHealth); 
    }
    public float Shields 
    { 
        get => shields; 
        set => shields = Mathf.Min(value, maxShields); 
    }

    public float MaxHealth 
    { 
        get => maxHealth; 
        set => maxHealth = value; 
    }

    public float MaxShields 
    { 
        get => maxShields; 
        set => maxShields = value; 
    }

    public float PlayerSpeed 
    { 
        get => playerSpeed; 
        set => playerSpeed = value; 
    }

    public float SneakSpeed 
    { 
        get => sneakSpeed; 
        set => sneakSpeed = value; 
    }

    public float DashSpeed 
    { 
        get => dashSpeed; 
        set => dashSpeed = value; 
    }

    public float OriginalSpeed 
    { 
        get => originalSpeed; 
        set => originalSpeed = value; 
    }

    public int PlayerNoise 
    { 
        get => playerNoise; 
        set => playerNoise = value; 
    }

    public int IchorSamples 
    { 
        get => ichorSamples; 
        set => ichorSamples = value;
    }

    public int TeleportationShardCount
    {
        get => teleporterShardCount;
        set => teleporterShardCount = value;
    }

    private void Start()
    {
        hitEvents = GetComponent<OnHitEvents>();
        if (hitEvents != null)
        {
            hitEvents.OnHarmed.AddListener(TakeDamage);
            hitEvents.OnStunned.AddListener(StunPlayer);
        }

        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController component is missing!");
        }


    }

    private void RevivePlayer()
    {
        Inventory inventory = GetComponent<Inventory>();
        EntityInfo entityInfo = GetComponent<EntityInfo>();
        if (inventory != null && entityInfo != null)
        {
            foreach (Item item in inventory.GetItems())
            {
                if (item != null && item.name == "Revivor" && item.CanUseItem())
                {
                    item.Use(entityInfo);
                    health = 1f; // Revive player with 1 health
                    Debug.Log("Player revived with Revivor item.");
                    break;
                }
            }
        }
    }

    private void Update()
    {
        MenuStates deathMenu = FindObjectOfType<MenuStates>();

        if (deathMenu == null)
            Debug.Log("Coudl not find death menu");

        if (health <= 0)
        {
            RevivePlayer();

            if (health <= 0)
            {
                Debug.Log("Player has died.");
                Destroy(gameObject);
                if (deathMenu != null)
                    deathMenu.ShowDeathMenu();
                //probably have code how to different menu upon player death
                
            }
        }
        // THIS IS TESTING, THIS CAN BE REMOVED ONCE
        // WE GET ON HIT DETECTION WORKING
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("damaged the player");
            TakeDamage(1f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (playerController != null && !playerController.isInvincible)
        {
            if (Shields > 0)
            {
                Shields -= Mathf.Min(damage,shields); //so the shield will save player from fatal damage
            }
            else
            {
                Health -= damage; // damage to the player
            }

            HUD hud = FindObjectOfType<HUD>();
            if (hud.intSlider != null)
            {
                hud.SetHealthBar(shields/maxShields);
                Debug.Log($"New health bar value: {hud.intSlider.value}");
            }
            else
            {
                Debug.LogError("hud reference isnt working in PlayerStats :c");
            }

        }
        else
        {
            Debug.Log("Player is invincible and did not take damage.");
            return;
        }
    }

    public void RechargeShields(float charges)
    {
        shields += charges/4; // shields has 4 segments

        HUD hud = FindObjectOfType<HUD>();
        if (hud.intSlider != null)
        {
            hud.SetHealthBar(Shields/maxShields);
            Debug.Log($"New health bar value: {hud.intSlider.value}");
        }
        else
        {
            Debug.LogError("hud reference isnt working in PlayerStats :c");
        }
    }

    public void StunPlayer(float stunDuration)
    {
        if (playerController != null)
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }
    }

    private IEnumerator StunCoroutine(float stunDuration)
    {
        playerController.isParalyzed = true;
        yield return new WaitForSeconds(stunDuration);
        playerController.isParalyzed = false;
    }
}
