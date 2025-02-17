using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// A collection of static functions which might be helpful to have in any state
public static class MonsterHelper
{
    /*
    * TITLE : How to get a random point on NavMesh?
    * AUTHOR : Selzier, Valkyr_x
    * DATE : 2/8/2025
    * AVAILABIILTY : https://discussions.unity.com/t/how-to-get-a-random-point-on-navmesh/73440/2
    */
    public static Vector3 RandomNavmeshLocation(float radius, Vector3 currentPosition)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += currentPosition;
        NavMeshHit hit;
        Vector3 finalPosition = currentPosition;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
