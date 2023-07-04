using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class ShopAsset : MonoBehaviour
{
    private static ShopAsset _i;

    public static ShopAsset i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<ShopAsset>("ShopAsset"));
            return _i;
        }
    }

    public Sprite Weapon1;
    public Sprite Weapon2;
    public Sprite weapon3;
    public Sprite Potion;
}
