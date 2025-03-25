/*
File: MonsterController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

[DefaultExecutionOrder(-5)]
public class MonsterController : MonoBehaviour
{
    [SerializeField] OnHitEvents hitEvents;
    [SerializeField] OnInteractionEvent interactionEvent;
    [SerializeField] MonsterSightEvents sightEvents;
    [SerializeField] SimpleSightMeshController sightController;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Transform spriteTransform;
    [SerializeField] protected bool facesRight = false;
    [SerializeField] protected bool hasDeathAnimation = false;
    [SerializeField] protected MonsterState state;

    [Header("Stats")]
    public float HearingAmplificaiton = 1f;
    public float VisionTurnRate = 90f; // degrees / sec
    public bool useVisionSmoothing = false;


    [SerializeField] protected float hp;
    [SerializeField] protected float maxHP;
    protected float stunDuration;

    protected float baseSpeed;
    protected MonsterState baseState;
    protected Vector3 spawnPoint;

    protected List<GameObject> objectsInView = new List<GameObject>();
    protected bool overrideSightDirection = false;
    protected float TargetLookDir = -90f;

    protected const float RESPAWN_DELAY = 30f;

    protected Vector3 baseSpritePos;
    protected Vector3 baseSpriteScale;

    public NavMeshAgent Agent { get => agent; }
    public List<GameObject> ObjectsInView { get => objectsInView; }
    public float HP { get => hp; set => hp = value; }
    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float BaseSpeed { get => baseSpeed; set => baseSpeed = value; }
    public bool OverrideSightDirection { get => overrideSightDirection; set => overrideSightDirection = value; }
    public SimpleSightMeshController SightController { get => sightController; }


    protected void Awake()
    {
        if (state != null)
        {
            state.controller = this;
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        baseSpeed = Agent.speed; // grab our speed before the state changes it
        baseState = state;
        spawnPoint = transform.position;

        if (spriteTransform != null)
        {
            baseSpritePos = spriteTransform.localPosition;
            baseSpriteScale = spriteTransform.localScale;
        }
    }

    private void OnEnable()
    {
        agent.enabled = true;
        agent.ResetPath();

        if (hitEvents != null)
        {
            hitEvents.OnStunned.AddListener(OnStunned);
            hitEvents.OnHarmed.AddListener(OnHarmed);
        }
        if (interactionEvent != null)
        {
            interactionEvent.OnInteract.AddListener(OnInteract);
        }
        if (sightEvents != null)
        {
            sightEvents.OnSeeingEntityEnterEvent.AddListener(OnSeeingEntityEnter);
            sightEvents.OnSeeingEntityExitEvent.AddListener(OnSeeingEntityExit);
        }
        NoiseDetectionManager.Instance.NoiseEvent.AddListener(OnNoiseDetection);
    }

    private void OnDisable()
    {
        if (hitEvents != null)
        {
            hitEvents.OnStunned.RemoveListener(OnStunned);
            hitEvents.OnHarmed.RemoveListener(OnHarmed);
        }
        if (interactionEvent != null)
        {
            interactionEvent.OnInteract.RemoveListener(OnInteract);
        }
        if (sightEvents != null)
        {
            sightEvents.OnSeeingEntityEnterEvent.RemoveListener(OnSeeingEntityEnter);
            sightEvents.OnSeeingEntityExitEvent.RemoveListener(OnSeeingEntityExit);
        }
        NoiseDetectionManager.Instance.NoiseEvent.RemoveListener(OnNoiseDetection);
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateStunDuration();
        if (overrideSightDirection == false)
        {
            LookTowardsPath();
        }
        
        if(stunDuration <= 0 && useVisionSmoothing) {
            TurnVision();
            
        if (animator != null)
        {
            animator.SetBool("isMoving", agent.velocity.magnitude > 0);
        }

        if (spriteTransform != null)
        {
            // mirror x
            if ((agent.velocity.x > 0 && facesRight == false) || // going right and default left
                (agent.velocity.x < 0 && facesRight == true)) // going left and default right
            {
                spriteTransform.localPosition = new Vector3(-baseSpritePos.x, baseSpritePos.y, baseSpritePos.z);
                spriteTransform.localScale = new Vector3(-baseSpriteScale.x, baseSpriteScale.y, baseSpriteScale.z);
            }
            else // original x
            {
                spriteTransform.localPosition = baseSpritePos;
                spriteTransform.localScale = baseSpriteScale;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) // OnTouchEnter
    {
        state.OnTouchEnter(collision);
    }

    private void OnCollisionExit2D(Collision2D collision) // OnTouchExit
    {
        state.OnTouchExit(collision);
    }

    private void OnSeeingEntityEnter(Collider2D collider)
    {
        if (collider.gameObject != gameObject)
        {
            objectsInView.Add(collider.gameObject);
            state.OnSeeingEntityEnter(collider);
        }
    }

    private void OnSeeingEntityExit(Collider2D collider)
    {
        if (collider.gameObject != gameObject)
        {
            objectsInView.Remove(collider.gameObject);
            state.OnSeeingEntityExit(collider);
        }
    }

    private void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        float distance = Vector3.Distance(transform.position, pos);
        float amplifiedVolume = volume * HearingAmplificaiton;
        if (amplifiedVolume > distance)
        {
            state.OnNoiseDetection(pos, volume, tags);
        }
    }

    public void SwitchState<T>()
    {
        T stateComponent = gameObject.GetComponent<T>();
        if (typeof(T).IsSubclassOf(typeof(MonsterState)) == false)
        {
            Debug.LogError($"MonsterController was told to switch the state to a non state component \nComponent in question is {typeof(T)}");
            return;
        }

        // disabled the old state
        state.enabled = false;

        // enable the new state
        state = stateComponent as MonsterState;
        state.controller = this;
        state.enabled = true;
    }

    protected void UpdateStunDuration()
    {
        if (stunDuration > 0)
        {
            stunDuration -= Time.deltaTime;
            if (stunDuration < 0) // clear negative stun durations
            {
                OnStunEnd();
            }
        }
    }

    protected void LookTowardsPath()
    {
        // look at the next point in the navigation path, which is index 1
        if(agent.path.corners.Length > 1) {
            LookAt(agent.path.corners[1]);
        }
    }

    public void LookAt(Vector3 pos) {
        if (sightController != null) 
        {
            Vector3 direction = pos - transform.position;
            if(useVisionSmoothing) {
                TargetLookDir = SightMeshController.GetAngleFromVectorFloat(direction) + 90f;
            } else {
                sightController.LookDirection = SightMeshController.GetAngleFromVectorFloat(direction) + 90f;
            }
        }
    }

    // turn towards target vision
    protected void TurnVision() {
        var turnStep = VisionTurnRate*Time.deltaTime;
        if (sightController != null) 
        {
            if(sightController.LookDirection <= TargetLookDir-turnStep) {
                sightController.LookDirection += turnStep;
            } else if(sightController.LookDirection >= TargetLookDir+turnStep) {
                sightController.LookDirection -= turnStep;
            }
        }
    }

    protected void OnStunned(float duration)
    {
        stunDuration = Mathf.Max(duration, stunDuration);
        if (stunDuration > 0)
        {
            state.OnStunned(duration); // allow any final reactions before disabling the state
            OnStunStart();
        }
    }

    protected void OnHarmed(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            if (hasDeathAnimation)
            {
                animator.SetTrigger("death");
            }
            else
            {
                OnDeath();
            }
        }
        else
        {
            state.OnHarmed(damage); // do not react to the onHarm if the monster is dead
        }
    }

    protected void OnStunStart()
    {
        state.enabled = false;
        if (sightController != null) sightController.gameObject.SetActive(false);
        agent.speed = 0;
    }

    protected void OnStunEnd()
    {
        stunDuration = 0;
        if (sightController != null) sightController.gameObject.SetActive(true);
        state.enabled = true;
        agent.speed = baseSpeed;
    }

    public void OnDeath()
    {
        OnStunStart();
        gameObject.SetActive(false);
        
        RespawnManager.Instance.StartRespawnTimer(this, RESPAWN_DELAY);
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        hp = maxHP;
        state = baseState;
        
        OnStunEnd(); // ensure there is no remaining stun on the 
        gameObject.SetActive(true);
    }

    protected void OnInteract(GameObject other)
    {
        state.OnInteract(other);
    }






    public static List<Vector3> GenerateNavigationPoints(float navigationPointDistance)
    {
        List<Vector3> navigationPoints = new List<Vector3>();
        NavMeshPlus.Components.NavMeshSurface navSurface = GameObject.FindFirstObjectByType<NavMeshPlus.Components.NavMeshSurface>();
        // Make points spread out evenly across the entire navmesh 
        Bounds bounds = navSurface.navMeshData.sourceBounds;
        int xPoints = Mathf.CeilToInt(bounds.size.x / navigationPointDistance);
        int yPoints = Mathf.CeilToInt(bounds.size.z / navigationPointDistance); // navmesh needs to be rotated, so this needs to be z, not y

        float xDifference = bounds.size.x / (xPoints + 1);
        float yDifference = bounds.size.z / (yPoints + 1);

        for (int x = 0; x <= xPoints; x++)
        {
            for (int y = 0; y <= yPoints; y++)
            {
                Vector3 rawNavPoint = new Vector3(bounds.min.x, bounds.min.z, 0) + new Vector3(xDifference / 2 + xDifference * x, yDifference / 2 + yDifference * y, 0);
                if (NavMesh.SamplePosition(rawNavPoint, out NavMeshHit hit, navigationPointDistance, 1))
                {
                    navigationPoints.Add(hit.position);
                }
            }
        }

        return navigationPoints;
    }
}
