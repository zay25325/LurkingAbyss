using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSightEvents : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Collider2D> OnSeeingEntityEnterEvent = new UnityEvent<Collider2D>();
    [HideInInspector] public UnityEvent<Collider2D> OnSeeingEntityExitEvent = new UnityEvent<Collider2D>();
    [SerializeField] LayerMask raycastLayers;

    List<GameObject> objectInFov = new List<GameObject>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 direction = (collision.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (direction * 0.5f), direction, 5, raycastLayers);
        if (hit.collider != collision) // if we cant see the object
        {
            if (objectInFov.Contains(collision.gameObject))
            {
                objectInFov.Remove(collision.gameObject);
                OnSeeingEntityExitEvent.Invoke(collision);
            }
        }
        else // if we can see the object
        {
            if (objectInFov.Contains(collision.gameObject) == false)
            {
                objectInFov.Add(collision.gameObject);
                OnSeeingEntityEnterEvent.Invoke(collision);
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    OnSeeingEntityEnterEvent.Invoke(collision);
    //}

    private void OnTriggerExit2D(Collider2D collision)
    {
        // only remove the object if we could see it before
        if (objectInFov.Contains(collision.gameObject))
        {
            objectInFov.Remove(collision.gameObject);
            OnSeeingEntityExitEvent.Invoke(collision);
        }
    }
}
