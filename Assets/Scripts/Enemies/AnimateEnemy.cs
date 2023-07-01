using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        //Load Component
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        //Subscribe to Idle Event
       // enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        //Subscribe to Idle Event
       // enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    //On movement Event Handler
    private void MovementToPositionEvent_OnMovementToPosition (MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs)
    {
        MoveAnimate();
    }

    private void MoveAnimate()
    {
        if (enemy.transform.position.x < GameManager.Instance.GetPlayer().transform.position.x)
        {
            enemy.spriteRenderer.flipX = false;
        }
        else
        {
            enemy.spriteRenderer.flipX = true;
        }

        enemy.animator.SetBool("isMoving", true);

        if (enemy.transform.position.y < GameManager.Instance.GetPlayer().transform.position.y)
        {
            enemy.animator.SetBool("front", false);
        }
        else
        {
            enemy.animator.SetBool("front", true);
        }
    }

}
