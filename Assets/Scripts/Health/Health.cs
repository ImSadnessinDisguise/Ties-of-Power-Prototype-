using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHeath;
    public int health;
    public int numOfHearts;

    public Image  heartImage;
    public Sprite fullHeart, halfHeart, emptyHeart;

    public enum HeartStatus
    {
        Empty = 0,
        Half = 1,
        Full = 2
    }

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    
    public void SetHeartImage(HeartStatus status)
    {
        switch (status)
        {
            case HeartStatus.Empty:
                heartImage.sprite = emptyHeart;
                break;

            case HeartStatus.Half:
                heartImage.sprite = halfHeart;
                break;

            case HeartStatus.Full:
                heartImage.sprite = fullHeart;
                break;
        }
    }


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
