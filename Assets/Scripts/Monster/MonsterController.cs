using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

public class MonsterController : MonoBehaviour
{
    [SerializeField] OnHitEvents hitEvents;
    [SerializeField] OnInteractionEvent interactionEvent;
    [SerializeField] MonsterSightEvents sightEvents;
    [SerializeField] SightMeshController sightController;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected MonsterState state;

    [Header("Stats")]
    public float HearingAmplificaiton = 1f;

    [SerializeField] protected float hp;
    [SerializeField] protected float maxHP;
    protected float stunDuration;

    protected float baseSpeed;
    protected MonsterState baseState;
    protected Vector3 spawnPoint;

    protected List<GameObject> objectsInView = new List<GameObject>();
    protected bool overrideSightDirection = false;

    protected const float RESPAWN_DELAY = 5f;

    public NavMeshAgent Agent { get => agent; }
    public List<GameObject> ObjectsInView { get => objectsInView; }
    public float HP { get => hp; set => hp = value; }
    public float MaxHP { get => maxHP; set => maxHP = value; }
    public float BaseSpeed { get => baseSpeed; set => baseSpeed = value; }


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
    }

    private void OnEnable()
    {
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
        if (sightController != null && agent.path.corners.Length > 1) 
        {
            Vector3 direction = agent.path.corners[1] - transform.position;
            sightController.LookDirection = SightMeshController.GetAngleFromVectorFloat(direction) + 90f;
        }
    }

    protected void OnStunned(float duration)
    {
        stunDuration = Mathf.Max(duration, stunDuration);
        if (stunDuration > 0)
        {
            OnStunStart();
        }
    }

    protected void OnHarmed(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            OnDeath();
        }
    }

    protected void OnStunStart()
    {
        state.enabled = false;
        sightController.gameObject.SetActive(false);
        agent.speed = 0;
    }

    protected void OnStunEnd()
    {
        stunDuration = 0;
        sightController.gameObject.SetActive(true);
        state.enabled = true;
        agent.speed = baseSpeed;
    }

    protected void OnDeath()
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
}
