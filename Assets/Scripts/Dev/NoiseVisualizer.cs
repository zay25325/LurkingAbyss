using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseVisualizer : MonoBehaviour
{
    public float volume;
    private float decay;
    public Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position.Set(this.position.x,this.position.y,0);
    }

    // Update is called once per frame
    void Update()
    {
        decay += 3*Time.deltaTime;
        volume -= (decay)*(volume)*Time.deltaTime;
        if(volume <= 0.1) {
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0f,1f,1f,0.2f);
        Gizmos.DrawSphere(new Vector3(this.position.x,this.position.y,0), volume);
    }
}