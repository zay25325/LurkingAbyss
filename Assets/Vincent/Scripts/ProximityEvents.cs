using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// this class gets entityinfo of entities that enter a radius

public class ProximityEvents : MonoBehaviour
{
    [HideInInspector] public UnityEvent<EntityInfo> OnEnter = new UnityEvent<EntityInfo>();
    [HideInInspector] public UnityEvent<EntityInfo> OnExit = new UnityEvent<EntityInfo>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var tags = collision.gameObject.GetComponent<EntityInfo>();
        if(tags != null) OnEnter.Invoke(tags);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var tags = collision.gameObject.GetComponent<EntityInfo>();
        if(tags != null) OnExit.Invoke(tags);
    }


}
