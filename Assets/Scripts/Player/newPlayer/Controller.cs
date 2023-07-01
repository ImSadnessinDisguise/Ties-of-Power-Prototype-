using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(mainPlayer))]
public class Controller : MonoBehaviour
{
    public Rigidbody2D body;
    public SpriteRenderer spRenderer;
    public areaAttack attackArea;
    private bool attacking = false;
    public Animator animator;
    
    public LayerMask enemyLayers;
    public GameObject areaHitbox;
    private mainPlayer player;
    private WaitForFixedUpdate waitForFixedUpdate;

    public float walkSpeed;
    public float framRate;

    Vector2 direction;

    private void Awake()
    {

        animator = GetComponent<Animator>();
        player = GetComponent<mainPlayer>();

    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Update()
    {
        Move();

        SpriteFlip();

        UseItemInput();

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
        //attackArea.SetActive(attacking);

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
            attackArea.attackDirection = areaAttack.AttackDirection.left;
        }
        else if (spRenderer.flipX && direction.x > 0)
        {
            spRenderer.flipX = false;
            attackArea.attackDirection = areaAttack.AttackDirection.right;
        }
    }


    private void UseItemInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float useItemRadius = 2f;

            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

            foreach (Collider2D collider2D in collider2DArray)
            {
                IUseable iUseable = collider2D.GetComponent<IUseable>();

                if (iUseable != null)
                {
                    iUseable.UseItem();
                }
            }
        }
    }

}
