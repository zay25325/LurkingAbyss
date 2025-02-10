using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideInPlayMode : MonoBehaviour
{
    [SerializeField] public bool Hide = false;

    private void Start() {
        if(Hide) {
            var renderer = GetComponent<Renderer>();
            renderer.enabled = false;
        }
    }
}
