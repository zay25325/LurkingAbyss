using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSightEvents : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collider2D> OnSeeingEntityEnterEvent = new UnityEvent<Collider2D>();
    [HideInInspector] public UnityEvent<Collider2D> OnSeeingEntityExitEvent = new UnityEvent<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnSeeingEntityEnterEvent.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnSeeingEntityExitEvent.Invoke(collision);
    }
}
