using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapperExplosiveController : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Vector3 explosiveOffset = new Vector3(0, 0.55f, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityInfo info = collision.gameObject.GetComponent<EntityInfo>();
        if (info != null && info.Tags.Contains(EntityInfo.EntityTags.Trapper) == false)
        {
            Instantiate(explosionPrefab, transform.position + explosiveOffset, new Quaternion());
            GameObject.Destroy(gameObject);
        }
    }
}
