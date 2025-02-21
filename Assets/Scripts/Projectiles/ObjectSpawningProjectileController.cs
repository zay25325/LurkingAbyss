using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawningProjectileController : EarlyStopProjectileController
{
    [SerializeField] GameObject prefab;

    override protected void OnFlightEnd()
    {
        GameObject spawnedObj = GameObject.Instantiate(prefab); // TODO: this spawn it at 0,0 before we move it. check that this doesn't hit stuff at 0,0
        spawnedObj.transform.position = transform.position;
        base.OnFlightEnd();
    }
}
