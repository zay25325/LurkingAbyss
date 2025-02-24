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

    public SpriteRenderer spriteRenderer;

    public Sprite OriginalSprite;

    public float AttackDistance { get => attackDistance; }
    public float AttackCooldown { get => attackCooldown; }
    public float AttackDuration { get => attackDuration; }
    public float AttackDamage { get => attackDamage; }

    public List<Item> items = new List<Item>();

    public void SetItemSprite(Sprite itemSprite)
    {
        StartCoroutine(PlayMimicLickAnimation(itemSprite));
    }

    private IEnumerator PlayMimicLickAnimation(Sprite itemSprite)
    {
        // Play the "mimic lick" animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = true;
                    // Wait until the animation is no longer playing
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }

        animator.enabled = false;
        // After the animation, set the item sprite
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
