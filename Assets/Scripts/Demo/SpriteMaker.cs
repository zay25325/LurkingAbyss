using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaker : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector2> points = new List<Vector2>
        {
            new Vector2(-1,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(-1,-1),
        };
    }
}
