using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapperBulbController : MonoBehaviour
{
    [SerializeField] OnHitEvents onHitEvents;
    [SerializeField] GameObject ColliderObj;

    TrapperController trapper;
    bool isBulbActive = true;

    // Start is called before the first frame update
    void Start()
    {
        onHitEvents.OnHarmed.AddListener(OnHarmed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBulbActive == true && trapper != null)
        {
            EntityInfo info = collision.GetComponent<EntityInfo>();
            if (info != null && 
                info.Tags.Contains(EntityInfo.EntityTags.Creature) && 
                info.Tags.Contains(EntityInfo.EntityTags.Trapper) == false)
            {
                trapper.OnTrapTriggered(this, collision.transform.position);
            }
        }
    }

    private void OnHarmed(float damage)
    {
        if (damage > 0)
        {
            TriggerTrapDestroyed();
        }
    }

    public void SetTrapper(TrapperController trapper)
    {
        this.trapper = trapper;
    }

    public void TriggerTrapDestroyed()
    {
        trapper.OnTrapDestroyed(this);
        ColliderObj.SetActive(false);
    }

    public void TriggerTrapRestored()
    {
        trapper.OnTrapRestored(this);
        ColliderObj.SetActive(true);
    }
}
