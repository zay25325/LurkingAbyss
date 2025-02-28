/*
File: ProjectileController.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 2/28/2025
*/

using UnityEngine;
using UnityEngine.Events;

public class ProjectileEventModifier : ProjectileModifier
{

    public UnityEvent<Vector2, Vector2, Vector2> MoveEvent = new UnityEvent<Vector2, Vector2, Vector2>();
    public UnityEvent<Collider2D> HitEvent = new UnityEvent<Collider2D>();
    public UnityEvent EndOfFlightEvent = new UnityEvent();

    public override void OnMove(Vector2 currentPosition, Vector2 StartPosition, Vector2 TargetPosition)
    {
        MoveEvent.Invoke(currentPosition, StartPosition, TargetPosition);
    }

    public override void OnHit(Collider2D collision)
    {
        HitEvent.Invoke(collision);
    }

    public override void OnEndOfFlight()
    {
        EndOfFlightEvent.Invoke();
    }
}
