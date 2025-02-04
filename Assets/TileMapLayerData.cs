using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapLayer : MonoBehaviour
{
    [SerializeField] public enum LayerClass {
        Floor,
        Wall,
        Palette,
        Creep,
        None,
    }

    [SerializeField] public LayerClass id = LayerClass.None;
}
