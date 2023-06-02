using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHeath;

    ///<summary>
    ///Set Starting Health
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHeath = startingHealth;
    }

    ///<summary>
    ///Get Starting Health
    /// </summary>
    public int GetStartingHealth()
    {
        return startingHealth;
    }




}
