using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KerisAttack : MonoBehaviour
{
    Vector2 rightAttackOffset;
    Collider2D kerisCollider;

    public enum AttackDirection
    {
        left, right
    }

    public AttackDirection attackDirection;

    private void Start()
    {
        kerisCollider = GetComponent<Collider2D>();
        rightAttackOffset = transform.position;
    }

    private void Attack()
    {
        switch (attackDirection)
        {
            case AttackDirection.left:
                 AttackLeft();
                break;

            case AttackDirection.right:
                AttackRight();
                break;
        }
    }

    public void AttackRight()
    {
        kerisCollider.enabled = true;
        transform.position = rightAttackOffset;
    }

    public void AttackLeft()
    {
        kerisCollider.enabled = true;
        transform.position = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    private void StopAttack()
    {
        kerisCollider.enabled = false;
    }
}
