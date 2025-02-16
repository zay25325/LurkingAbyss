using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnHitEvents : MonoBehaviour
{
    [HideInInspector] public UnityEvent<float> OnStunned = new UnityEvent<float>();
    [HideInInspector] public UnityEvent<float> OnHarmed = new UnityEvent<float>();
    [HideInInspector] public UnityEvent<float> OnStructuralDamaged = new UnityEvent<float>();

    public void ApplyHit(float stunDuration = 0, float damage = 0, float structuralDamaged = 0)
    {
        if (stunDuration > 0)
        {
            OnStunned.Invoke(stunDuration);
        }
        if (damage > 0)
        {
            OnHarmed.Invoke(damage);
        }
        if (structuralDamaged > 0)
        {
            OnStructuralDamaged.Invoke(structuralDamaged);
        }
    }
}
