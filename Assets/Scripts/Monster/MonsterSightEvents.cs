using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSightEvents : MonoBehaviour
{
    List<Collider> colliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"new trigger entered: {collision.name}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"new trigger exited: {collision.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"new trigger entered: {other.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"new trigger exited: {other.name}");
    }
}
