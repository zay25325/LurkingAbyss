using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwarmlingController : MonsterController
{
    private float baseSpeed;
    public float BaseSpeed { get => baseSpeed; }
    private float fleeDistance = 8f;

    private CircleCollider2D touchCollider;

    new protected void Awake()
    {
        base.Awake(); // set up the first state
        baseSpeed = Agent.speed; // grab our speed before the state changes it
        touchCollider = GetComponent<CircleCollider2D>();
    }

    public void FleeFromSound(Vector2 pos, bool switchToFleeing = true)
    {
        Vector2 direction = (pos - (Vector2)transform.position).normalized * -1; // direction away from sound
        Vector2 rayStart = transform.position + (Vector3)direction * touchCollider.radius;

        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, fleeDistance); // hit wall
        Vector3 fleePoint = transform.position + (Vector3)(direction * fleeDistance); // were to flee to
        if (hit.collider != null)
        {
            fleePoint = hit.point;
        }

        Vector3 navPoint = transform.position; // default to current position in case of error
        if (NavMesh.SamplePosition(fleePoint, out NavMeshHit navHit, 1, 1)) // we will have hit a wall or a valid point, so we a radius of 1 is fine
        {
            navPoint = navHit.position;
        }

        Agent.destination = navPoint;
        if (switchToFleeing)
        {
            SwitchState<SwarmlingFleeState>();
        }
    }
}
