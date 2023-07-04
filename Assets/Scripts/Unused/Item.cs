using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType
    {
        Weapon1,
        Weapon2,
        Weapon3,
        HealthPotion
    }

    public static int GetCost(ItemType itemType)
    {
        switch(itemType)
        {
            default:
            case ItemType.Weapon1:
                return 199;

            case ItemType.Weapon2:
                return 599;

            case ItemType.Weapon3:
                return 899;

            case ItemType.HealthPotion:
                return 50;
        }
    }

    public static Sprite GetSprite(ItemType itemType)
    {
        switch(itemType)
        {
            default:
            case ItemType.Weapon1:
                return ShopAsset.i.Weapon1;

            case ItemType.Weapon2:
                return ShopAsset.i.Weapon2;

            case ItemType.Weapon3:
                return ShopAsset.i.weapon3;

            case ItemType.HealthPotion:
                return ShopAsset.i.Potion;
        }
    }
}
    

