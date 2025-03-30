using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapLayer : MonoBehaviour
{
    [SerializeField] public enum LayerClass {
        Floor,
        Wall,
        Corners,
        Unbreakable,
        Palette,
        Roomx9,
        Creep,
        None,
    }

    // each layer class has a purpose, and specific properties
    [SerializeField] public LayerClass id = LayerClass.None;

    // if it is part of the world, IE is it destroyed by nuke();
    [SerializeField] public bool IsWorld = true;
}
