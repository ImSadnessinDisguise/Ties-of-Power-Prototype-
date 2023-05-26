using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private GameObject attackArea;
    private bool isRolling = false;

    private bool attacking = false;
    public Animator anim;

    private float timetoAttack = 0.25f;
    private float timer = 0f;
    private float timetoRoll = 0.30f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        attackArea = transform.GetChild(0).gameObject;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
            anim.Play("attack");
        }

        if (attacking)
        {
            timer += Time.deltaTime;

            if (timer >= timetoAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
                
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Dodge();
            anim.Play("Roll");
        }
        if (isRolling)
        {
            timer += Time.deltaTime;

            if (timer >= timetoRoll)
            {
                timer = 0;
                isRolling = false;
            }
        }



    }

    private void Attack()
    {
        attacking = true;
        attackArea.SetActive(attacking);
    }

    private void Dodge()
    {
        isRolling = true;
    }

}
