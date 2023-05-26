using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; }

    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public CombatState combatState { get; private set; }

    public Animator Anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer spriteRenderer;
    private Vector2 movement;
    public PlayerInputHandler InputHandler { get; private set; }

    [SerializeField] private PlayerData playerData;

    public void Awake()
    {
        StateMachine = new PlayerStateMachine();
        idleState = new IdleState(this, StateMachine, playerData, "idle");
        moveState = new MoveState(this, StateMachine, playerData, "move");
        combatState = new CombatState(this, StateMachine, playerData, "combat");
    }

    private void Start()
    {
        Anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InputHandler = GetComponent<PlayerInputHandler>();
        StateMachine.Initialize(idleState);

    }

    public void Update()
    {
        StateMachine.Currentstate.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.Currentstate.PhysicsUpdate();
    }

    public void MovePlayer()
    {
        //direction of input
        Vector2 movement = new Vector2(InputHandler.MovementInput.x, InputHandler.MovementInput.y).normalized;

        rb.velocity = movement * playerData.movementSpeed;

        if (!spriteRenderer.flipX && movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (spriteRenderer.flipX && movement.x > 0)
        {
            spriteRenderer.flipX = false;
        } 
    }

}
