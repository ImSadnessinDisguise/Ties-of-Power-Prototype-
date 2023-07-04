using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    private Animator animator;
    public Transform enemyTransform;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        enemyTransform = GameManager.Instance.GetComponentInParent<Transform>();
    }

    private void Update()
    {
        CheckDirection();
    }

    private void CheckDirection()
    {
        if (enemyTransform.position.x < 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x * -1, transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("attack");
        DealDamage(collision);
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(10);
        }
    }
}
