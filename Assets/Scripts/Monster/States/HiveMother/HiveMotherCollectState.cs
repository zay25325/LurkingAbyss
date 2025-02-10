using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiveMotherCollectState : MonsterState
{
    new private HiveMotherController controller { get => base.controller as HiveMotherController; }

    const float navUpdateLimit = .25f; // Don't recalculate a navigation path every single frame. That would be bad
    float navTimer = 0;

    private void Update()
    {
        // So that the destination would update repeatedly instead of only the first time a swarmling enter the FOV
        navTimer += Time.deltaTime;
        if (navTimer > navUpdateLimit)
        {
            navTimer = 0f; // reset timer
            for (int i = 0; i < controller.ObjectsInView.Count; i++) // see if there is a swarmling in our vision
            {
                GameObject obj = controller.ObjectsInView[i];
                EntityInfo info = obj.GetComponent<EntityInfo>();
                if (info != null && info.Tags.Contains(EntityInfo.EntityTags.Swarmling))
                {
                    Vector3 direction = (obj.transform.position - transform.position).normalized;
                    controller.Agent.SetDestination(obj.transform.position + direction); // agents slow down when reaching their destination, so intentionally overshoot
                    NoiseDetectionManager.Instance.NoiseEvent.Invoke(transform.position, Vector3.Distance(transform.position, obj.transform.position) + 1, GetComponent<EntityInfo>().Tags); ;
                    break;
                }
            }
        }

        // if the Hive Mother somehow misses the swarmlings (it could potentially die)
        if (controller.Agent.remainingDistance < .1f)
        {
            controller.SwitchState<HiveMotherCallState>();
        }
    }

    private void OnEnable()
    {
        navTimer = 0f;
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        EntityInfo info = collider.GetComponent<EntityInfo>();
        if (info != null && info.Tags.Contains(EntityInfo.EntityTags.Swarmling))
        {
        }
    }

    public override void OnTouchEnter(Collision2D collision)
    {
        EntityInfo info = collision.collider.GetComponent<EntityInfo>();
        if (info != null && info.Tags.Contains(EntityInfo.EntityTags.Swarmling))
        {
            info.gameObject.SetActive(false);
            GameObject.Destroy(info.gameObject);
            controller.CollectedSwarmlings++;
            controller.SwitchState<HiveMotherCallState>();
        }
    }
}
