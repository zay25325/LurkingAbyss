using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Events;
using NavMeshPlus;
using UnityEngine.AI;
using System;

public class DoorController : MonoBehaviour
{
    [SerializeField] private OnHitEvents hitEvents; //so the door can be destroyed by explosions
    [SerializeField] private ProximityEvents proximityEvents; // so the door can open for smart monsters and the player

    [SerializeField] bool isOpen = false;
    [SerializeField] float strength = 0; // only breaks when taking more than this amount of structural damage
    [SerializeField] private int entitiesInTrigger = 0; // to track whether it should be open or not
    [SerializeField] private Collider2D myCollider;
    [SerializeField] private SpriteRenderer myRenderer;

    [SerializeField] private NavMeshObstacle navModifierComponent;
    private int SmartMeshId; //id of the smartmonster navmesh so they can path through doors

    private static int debugcount = 0;

    private void Start() {
        
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++)
        {
            NavMeshBuildSettings settings = NavMesh.GetSettingsByIndex(index: i);
            if (name == NavMesh.GetSettingsNameFromID(agentTypeID: settings.agentTypeID))
            {
                SmartMeshId = settings.agentTypeID;
            }
        }

        this.myCollider = this.gameObject.GetComponent<Collider2D>();
        this.myRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();

        hitEvents.OnStructuralDamaged.AddListener(this.OnStructuralDamage);
        proximityEvents.OnEnter.AddListener(this.OnEnter);
        proximityEvents.OnExit.AddListener(this.OnLeave);
    }

    private void Update() {
    }

    private void Open() {
        this.isOpen = true;
        //disable the rigidbody collider
        this.myCollider.enabled = false;
        //update navmesh
        this.navModifierComponent.enabled = false;
        //black out the door to "open" it
        this.myRenderer.color = new Color(0f,0f,0f,0f);
    }

    private void Close() {
        this.isOpen = false;
        //disable the rigidbody collider
       this.myCollider.enabled = true;
        //update navmesh
        this.navModifierComponent.enabled = true;
        //black out the door to "open" it
        this.myRenderer.color = Color.white;
    }

    private void OnEnter(EntityInfo info) {
        if(info.Tags.Contains(EntityInfo.EntityTags.CanOpenDoors)) {
            entitiesInTrigger ++;
            if(entitiesInTrigger > 0 && isOpen == false) {
                this.Open();
            }
        }
    }

    private void OnLeave(EntityInfo info) {
        if(info.Tags.Contains(EntityInfo.EntityTags.CanOpenDoors)) {
            entitiesInTrigger --;
            if(entitiesInTrigger <= 0 && isOpen == true) {
                this.Close();
            }
        }
    }

    // boop
    private void OnStructuralDamage(float damage) {
        // this could be a durability value for the door
        if(damage > this.strength) {
            this.myCollider.enabled = false;
            //update navmesh
            this.navModifierComponent.enabled = false;
            //die
            Destroy(this.gameObject);
        }
    }

}
