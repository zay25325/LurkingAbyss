using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// The only purpose of this class is to map cardinal directions to an integer from 0 to 3
//
//
public class Directions
{
    private static readonly List<Vector2> cardinals = new() {Vector2.up,Vector2.down,Vector2.left,Vector2.right};

    public static Vector2 IntToVec(int index) {
        return index switch {
            0 => Vector2.up,
            1 => Vector2.down,
            2 => Vector2.left,
            3 => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public static int VecToInt(Vector2 dir) {
        for(int i = 0; i < 4; i++) {
            if(cardinals[i] == dir) return i;
        }
        throw new ArgumentOutOfRangeException();
    }
}
