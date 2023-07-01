using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areaAttack : MonoBehaviour
{
    public enum AttackDirection
    { 
        left, right
    }


    public float damage = 1;

    public AttackDirection attackDirection;
    public Controller playerControl;

    Vector2 rightAttackOffset;

    private void Awake()
    {
        
    }

    private void Start()
    {
        rightAttackOffset = transform.localPosition;
        playerControl = GameManager.Instance.GetPlayer().GetComponent<Controller>();
    }

    private void Update()
    {
        SetAttackDirection();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<newHealth>() != null)
        {
            newHealth health = collider.GetComponent<newHealth>();

            health.TakeDamage(damage);
        }
    }

    public void Attack()
    {
        switch(attackDirection)
        {
            case AttackDirection.left:
                AttackLeft();
                break;
            case AttackDirection.right:
                AttackRight();
                break;
        }
    }

    private void SetAttackDirection()
    {
        if (playerControl.spRenderer.flipX == true)
        {
            AttackLeft();
        }
        else
        {
            AttackRight();
        }
    }


    public void AttackRight()
    {
        transform.localPosition = rightAttackOffset;
    }    

    public void AttackLeft()
    {
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, transform.localPosition.y);
    }

}
