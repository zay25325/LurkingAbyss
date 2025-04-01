/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/21/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Name: ProjectileController
Purpose: A parent class for all projectiles. Contains information for it's starting pos, target, speed and flight time.
  Will delete the projectile after the flight time is up, dealing damage to anything it touches.
  All AOEs should be able to use this class directly unless they need secondary effects.
*/
public class ProjectileController : MonoBehaviour
{
    // These values should ONLY be set in the prefab inspector. You should not need to write these values after you create the projectile
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxFligtTime = 10f;



    private Vector2 target;
    private Vector2 startingPos;

    private Vector3 MovementVector;
    private float flightTime = 0f;
    private bool endFlight = false;

    private List<ProjectileModifier> projectileModifiers = new List<ProjectileModifier>();

    public float Speed { get => speed; set => speed = value; }
    public float MaxFlightTime { get => maxFligtTime; set => maxFligtTime = value; }

    public Vector2 Target { get => target; set => SetTarget(value); }
    public Vector2 StartPos { get => startingPos; }


    private void OnEnable()
    {
        startingPos = transform.position;
        UpdateMovement();
        projectileModifiers.Clear();
        projectileModifiers.AddRange(GetComponentsInChildren<ProjectileModifier>());
    }

    // Update is called once per frame
    protected void Update()
    {
        transform.position += MovementVector * speed * Time.deltaTime;
        flightTime += Time.deltaTime;

        projectileModifiers.ForEach((modifier) => {
            modifier.OnMove(transform.position, startingPos, target);
        });

        if (flightTime > maxFligtTime || endFlight)
        {
            OnFlightEnd();
        }
    }

    protected private void OnTriggerEnter2D(Collider2D collision)
    {
        projectileModifiers.ForEach((modifier) => {
            modifier.OnHit(collision);
        });
    }

    // To improve performance, calculate this value ahead of time.
    private void UpdateMovement()
    {
        MovementVector = (target - startingPos).normalized;
    }

    public void SetTarget(Vector2 target)
    {
        startingPos = transform.position;
        this.target = target;
        UpdateMovement();
    }

    virtual protected void OnFlightEnd()
    {
        projectileModifiers.ForEach((modifier) => {
            modifier.OnEndOfFlight();
        });

        gameObject.SetActive(false);
        Destroy(gameObject); // If we make a projectile pool, just delete this
    }

    public void EndFlight()
    {
        endFlight = true;
    }
}
