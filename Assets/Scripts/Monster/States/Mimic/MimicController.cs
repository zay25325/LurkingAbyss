using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MimicController : MonsterController
{

    [Header("Mimic")]
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] float attackDamage = 1f;

    [SerializeField] Sprite disguiseSprite;
    [SerializeField] Sprite attackSprite;

    private SpriteRenderer spriteRenderer;

    public float AttackDistance { get => attackDistance; }
    public float AttackCooldown { get => attackCooldown; }
    public float AttackDuration { get => attackDuration; }
    public float AttackDamage { get => attackDamage; }

    public Sprite DisguiseSprite { get => disguiseSprite; set => disguiseSprite = value; }
    public Sprite AttackSprite { get => attackSprite; set => attackSprite = value; }

    public void SetItemSprite(Sprite itemSprite)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        if (itemSprite != null)
        {
            Debug.Log("Setting item sprite");
            spriteRenderer.sprite = itemSprite;
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
            transform.localScale = new Vector3(2, 2, 2);

            if (circleCollider != null)
            {
                circleCollider.radius = Mathf.Max(itemSprite.bounds.size.x, itemSprite.bounds.size.y) / 2;
            }
        }
    }
}
