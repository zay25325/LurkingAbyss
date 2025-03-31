/*
Class Name: PlayerStats
Description: This class is a component of the player game object and represents the player's health and shields. 
             It contains the properties and methods specific to the player's health and shields.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private Coroutine stunCoroutine;

    [HideInInspector] public UnityEvent<float> OnShieldsChanged;
    [HideInInspector] public UnityEvent<int> UpdateTeleShardsUI;

    [SerializeField] GameObject deadPlayerPrefab;

    public float Health 
    { 
        get => health; 
        set => health = value; 
    }
    public float Shields 
    { 
        get => shields; 
        set => shields = value; 
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
        set {
            teleporterShardCount = value;
            UpdateTeleShardsUI.Invoke(teleporterShardCount);
        }
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
        if (health <= 0)
        {
            RevivePlayer();

            if (health <= 0)
            {
                Debug.Log("Player has died.");
                Instantiate(deadPlayerPrefab, transform.position, new Quaternion());
                Destroy(gameObject);

                //probably have code how to different menu upon player death
                
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (playerController != null && playerController.isInvincible)
        {
            Debug.Log("Player is invincible and did not take damage.");
            return;
        }
        else if (Shields > 0)
        {
            Shields -= Mathf.Min(damage,shields); //so the shield will save player from fatal damage
        }
        else
        {
            Health -= damage; // damage to the player
        }
        OnShieldsChanged.Invoke(shields);
    }

    public void RechargeShields(float charges)
    {
        shields += charges * maxShields/4;
        shields = Mathf.Max(shields, maxShields);
        OnShieldsChanged.Invoke(shields);
    }

    public void StunPlayer(float stunDuration)
    {
        if (playerController != null)
        {
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
                playerController.isParalyzed = false;
            }
            stunCoroutine = StartCoroutine(StunCoroutine(stunDuration));
        }
    }

    private IEnumerator StunCoroutine(float stunDuration)
    {
        if (playerController.isInvincible != true)
        {
            playerController.isParalyzed = true;
            yield return new WaitForSeconds(stunDuration);
            playerController.isParalyzed = false;
            stunCoroutine = null;
        }
    }
}
