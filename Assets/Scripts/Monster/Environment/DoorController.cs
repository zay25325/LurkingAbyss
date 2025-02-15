using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using NavMeshPlus;

public class DoorController : MonoBehaviour
{
    [SerializeField] OnHitEvents hitEvents; //so the door can be destroyed by explosions
    [SerializeField] NavMeshSurface dumbMonsterNav; // so the door can block dumb monsters
    [SerializeField] ProximityEvents proximityEvents; // so the door can open for smart monsters and the player

    [SerializeField] bool isOpen = false;
    [SerializeField] float strength = 0; // only breaks when taking more than this amount of structural damage
    [SerializeField] private int entitiesInTrigger = 0; // to track whether it should be open or not

    [SerializeField] private Behaviour navModifierComponent;

    private void Start() {
        hitEvents.OnStructuralDamaged.AddListener(this.OnStructuralDamage);
        proximityEvents.OnEnter.AddListener(this.OnEnter);
        proximityEvents.OnExit.AddListener(this.OnLeave);
    }

    private void Update() {
        if(entitiesInTrigger > 0 && isOpen == false) {
            this.Open();
        } else if(entitiesInTrigger <= 0 && isOpen == true) {
            this.Close();
        }
    }

    private void Open() {
        this.isOpen = true;
        //disable the rigidbody collider
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        //update navmesh
        this.navModifierComponent.enabled = false;
        LevelController.UpdateNavMesh.Invoke();
        //black out the door to "open" it
        this.gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(0f,0f,0f,0f);
    }

    private void Close() {
        this.isOpen = false;
        //disable the rigidbody collider
        this.gameObject.GetComponent<Collider2D>().enabled = true;
        //update navmesh
        this.navModifierComponent.enabled = true;
        LevelController.UpdateNavMesh.Invoke();
        //black out the door to "open" it
        this.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    private void OnEnter(EntityInfo info) {
        if(info.Tags.Contains(EntityInfo.EntityTags.CanOpenDoors)) {
            entitiesInTrigger ++;
        }
    }

    private void OnLeave(EntityInfo info) {
        if(info.Tags.Contains(EntityInfo.EntityTags.CanOpenDoors)) {
            entitiesInTrigger --;
        }
    }

    // boop
    private void OnStructuralDamage(float damage) {
        // this could be a durability value for the door
        if(damage > this.strength) {
            Destroy(this.gameObject);
        }
    }

}
