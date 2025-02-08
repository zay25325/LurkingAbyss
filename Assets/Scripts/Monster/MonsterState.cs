using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterState : MonoBehaviour
{
    [HideInInspector] public MonsterController controller;

    // For completeness: The full list of events that a monsters are suppose to react to also includes these 4 MonoBehaviour events
    /* Events from MonoBehavior
    Start()
    Update()
    OnEnable()
    OnDisable()
    */

    public virtual void OnTouchEnter(Collision2D collision) { }
    public virtual void OnTouchExit(Collision2D collision) { }
    public virtual void OnSeeingEntityEnter(Collider2D collider) { }
    public virtual void OnSeeingEntityExit(Collider2D collider) { }
    public virtual void OnNoiseDetection(Vector2 pos, float volume) { }
}
