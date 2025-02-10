using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateMonsterState : MonsterState
{
    void Start()
    {
        // nothing to do on Start
    }

    void Update()
    {
        // nothing to do on Update
    }

    private void OnEnable()
    {
        // nothing to do when enabled
    }

    private void OnDisable()
    {
        // nothing to do when disabled
    }

    public override void OnNoiseDetection(Vector2 pos, float volume, List<EntityInfo.EntityTags> tags)
    {
        // nothing to do when hearing sounds
    }

    public override void OnSeeingEntityEnter(Collider2D collider)
    {
        // nothing to do when seeing an entity
    }

    public override void OnSeeingEntityExit(Collider2D collider)
    {
        // nothing to do when losing sight of an entity
    }

    public override void OnTouchEnter(Collision2D collision)
    {
        // nothing to do when touching an entity
    }

    public override void OnTouchExit(Collision2D collision)
    {
        // nothing to do when stopped touching an entity
    }
}
