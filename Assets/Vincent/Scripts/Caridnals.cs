using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The only purpose of this class is to map cardinal directions to an integer from 0 to 3
//
//
public class Caridnals
{
    // cardinal direction to int
    public static readonly Dictionary<Vector2,int> cardToInt = new() {{Vector2.up,0},{Vector2.down,1},{Vector2.left,2},{Vector2.right,3}};

    // int to cardinal direction
    public static readonly List<Vector2> intToCard = new() {Vector2.up,Vector2.down,Vector2.left,Vector2.right};
}
