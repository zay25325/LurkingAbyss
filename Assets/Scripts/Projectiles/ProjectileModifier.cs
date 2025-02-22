using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModifier : MonoBehaviour
{
    protected ProjectileController controller;
    protected void OnEnable()
    {
        controller = gameObject.GetComponent<ProjectileController>();
    }
    public virtual void OnMove(Vector2 currentPosition, Vector2 StartPosition, Vector2 TargetPosition) { }
    public virtual void OnHit(Collider2D collision) { }
    public virtual void OnEndOfFlight() { }
}
