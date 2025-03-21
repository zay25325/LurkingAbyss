using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapperBulbController : MonoBehaviour
{
    [SerializeField] OnHitEvents onHitEvents;
    [SerializeField] GameObject ColliderObj;
    [SerializeField] Animator animator;

    TrapperController trapper;
    bool isBulbActive = true;

    public bool IsBulbActive
    {
        get => isBulbActive;
        set
        {
            if (value != isBulbActive) // only trigger on change
            {
                isBulbActive = value;
                if (isBulbActive == true)
                {
                    trapper.OnTrapRestored(this);
                }
                else
                {
                    trapper.OnTrapDestroyed(this);
                }

                ColliderObj.SetActive(isBulbActive);
                animator.SetBool("IsActive", isBulbActive);
            }
        }
    }

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
        IsBulbActive = false;
    }

    public void TriggerTrapRestored()
    {
        IsBulbActive = true;
    }
}
