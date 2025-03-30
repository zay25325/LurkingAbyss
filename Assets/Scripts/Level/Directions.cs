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

    public static Vector2 Rotate2D(Vector2 v, float angle) {
        return new Vector2(
            v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle),
            v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle)
        );
    }

    // vector2 ints are kinda dumb
    public static Vector2Int Rotate2DInt(Vector2Int v, float turns) {
        int sina;
        int cosa;
        switch(turns%4) {
            case 1:
                sina = 1;
                cosa = 0;
                break;
            case 2:
                sina = 0;
                cosa = -1;
                break;
            case 3:
                sina = -1;
                cosa = 0;
                break;
            default:
                return v;
        }

        return new Vector2Int(
            v.x * cosa - v.y * sina,
            v.x * sina + v.y * cosa
        );
    }
}
