using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    public PlayerData playerData;
    public Rigidbody rb;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
    }

    private void FixedUpdate()
    {

        
    }

    public void MovePlayer()
    {
        
    }
}
