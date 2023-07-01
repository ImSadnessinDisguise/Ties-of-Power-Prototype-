using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RangeEnemy : MonoBehaviour
{
    [HideInInspector] public Animator animator;

    public float startTimeBetweenShots;
    public GameObject projectile;
    private float timeBetweenShots;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        timeBetweenShots = startTimeBetweenShots;
    }

    private void Update()
    {
        FireWeapon();
    }

    private void FireWeapon()
    {
        if (timeBetweenShots <= 0)
        {
            animator.SetTrigger("attack");
            Instantiate(projectile, transform.position, Quaternion.Euler(-45, 0, 0));
            timeBetweenShots = startTimeBetweenShots;

        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

}
