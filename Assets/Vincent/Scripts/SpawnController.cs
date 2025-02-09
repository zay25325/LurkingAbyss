using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] GameObject spawnThis = null;

    // give the spawnpoint an object to instantiate
    public void SetObject(GameObject prefab) {
        this.spawnThis = prefab;
    }

    // default behaviour is to instantiate a copy of registered thing in worldspace
    // override this to do something different
    public void OnTrigger() {
        // instantiate an object
        if(spawnThis != null) {
            Instantiate(this.spawnThis, this.transform, true);
        }
    }

    // so we can see in editor only

    [SerializeField] Color debugColor = new(0.8f,0f,0f,0.5f);

    private void OnDrawGizmos() {
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(this.transform.position, 0.4f);
    }
}
