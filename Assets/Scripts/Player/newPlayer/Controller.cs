using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Rigidbody2D body;
    public SpriteRenderer spRenderer;
    private GameObject attackArea = default;
    private bool attacking = false;
    public Animator animator;
    public LayerMask enemyLayers;

    public float walkSpeed;
    public float framRate;

    Vector2 direction;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }
    private void Update()
    {
        Move();

        SpriteFlip();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();

        }

        

        if (direction.x > 0 || direction.x < 0)
        {
            animator.SetBool("move", true);
        }
        else
        {
            animator.SetBool("move", false);
        }

        if (direction.y < 0)
        {
            animator.SetBool("front", true);
        }
        else
        {
            animator.SetBool("front", false);
        }

        if (direction.y > 0)
        {
            animator.SetBool("back", true);
        }
        else
        {
            animator.SetBool("back", false);
        }

    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        attackArea.SetActive(attacking);

    }

    public void Move()
    {
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        body.velocity = direction * walkSpeed;
    }

    public void SpriteFlip()
    {
        //handle direction
        if (!spRenderer.flipX && direction.x < 0)
        {
            spRenderer.flipX = true;
        }
        else if (spRenderer.flipX && direction.x > 0)
        {
            spRenderer.flipX = false;
        }
    }

    public void SetSprite()
    {
        if (direction.y > 0 )
        {

        }
    }

}
