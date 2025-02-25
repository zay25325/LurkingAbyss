using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//
// This object is responsible for placing entities on level load
//
public class SpawnController : MonoBehaviour
{
    //public enum SpawnClass {
    //    Item,
    //    Interactable,
    //    Trap,
    //    Monster,
    //    None,
    //    PlayerStart,
    //    PlayerExit
    //}


    //[SerializeField] public GameObject spawnThis = null;
    //[SerializeField] public SpawnClass type = SpawnClass.None;

    //[SerializeField] public List<EntityInfo.EntityTags> tags = new() {};

    //[SerializeField] public UnityEvent trigger; // trigger what?

    //private void Start() {
    //    trigger.AddListener(OnTrigger);
    //}

    // give the spawnpoint an object to instantiate
    //public void AssignObject(GameObject prefab) {
    //    this.spawnThis = prefab;
    //}

    // default behaviour is to instantiate a copy of registered thing in worldspace
    // override this to do something different
    //public void OnTrigger() {
    //    // instantiate an object
    //    if(spawnThis != null) {
    //        Instantiate(this.spawnThis, this.transform, true);
    //    }
    //}


    // so we can see in editor only
    [SerializeField] Color debugColor = new(0.8f,0f,0f,0.5f);

    private void OnDrawGizmos() {
        Gizmos.color = debugColor;
        Gizmos.DrawSphere(this.transform.position+new Vector3(0.5f,0.5f,0f), 0.4f);
    }
}
