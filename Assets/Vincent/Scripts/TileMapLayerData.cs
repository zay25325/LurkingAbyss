using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapLayer : MonoBehaviour
{
    [SerializeField] public enum LayerClass {
        Floor,
        Wall,
        Unbreakable,
        Palette,
        Roomx9,
        Creep,
        None,
    }

    [SerializeField] public LayerClass id = LayerClass.None;
}
