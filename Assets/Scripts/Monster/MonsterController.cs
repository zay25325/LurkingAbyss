using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] MonsterSightEvents sightEvents;
    [SerializeField] SightMeshController sightController;
    [SerializeField] NavMeshAgent agent;

    public float HearingAmplificaiton = 1f;

    [SerializeField] protected MonsterState state;
    protected float stunDuration;
    protected List<GameObject> ObjectsInView = new List<GameObject>();

    protected bool overrideSightDirection = false;

    public NavMeshAgent Agent { get => agent; }

    protected void Awake()
    {
        state.controller = this;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        // prevent agent from messing with 2d visuals
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (sightEvents != null)
        {
            sightEvents.OnSeeingEntityEnterEvent.AddListener(OnSeeingEntityEnter);
            sightEvents.OnSeeingEntityExitEvent.AddListener(OnSeeingEntityExit);
        }
        NoiseDetectionManager.Instance.NoiseEvent.AddListener(OnNoiseDetection);
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
            ObjectsInView.Add(collider.gameObject);
            state.OnSeeingEntityEnter(collider);
        }
    }

    private void OnSeeingEntityExit(Collider2D collider)
    {
        if (collider.gameObject != gameObject)
        {
            ObjectsInView.Remove(collider.gameObject);
            state.OnSeeingEntityExit(collider);
        }
    }

    private void OnNoiseDetection(Vector2 pos, float volume)
    {
        float distance = Vector3.Distance(transform.position, pos);
        float amplifiedVolume = volume * HearingAmplificaiton;
        if (amplifiedVolume > distance)
        {
            state.OnNoiseDetection(pos, volume);
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
                stunDuration = 0;
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
}
