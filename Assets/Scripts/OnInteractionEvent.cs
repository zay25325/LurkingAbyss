using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnInteractionEvent : MonoBehaviour
{
    [HideInInspector] public UnityEvent<GameObject> OnInteract = new UnityEvent<GameObject>();
}
