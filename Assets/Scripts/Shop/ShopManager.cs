using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    private TeemarCount teemar;
    public Upgrade[] upgrades;

    //References
    public Text teemarText;
    public GameObject shopUI;
    public Transform shopContent;
    public GameObject itemPrefab;
    private Controller playerMove;
    private Health health;

    private void Awake()
    {

        playerMove = GetComponent<Controller>();
        health = GetComponent<Health>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (Upgrade upgrade in upgrades)
        {
            GameObject item = Instantiate(itemPrefab, shopContent);

            upgrade.itemRef = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "itemPrice")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = upgrade.cost.ToString();
                }
                else if (child.gameObject.name == "itemName")
                {
                    child.gameObject.GetComponent<TMP_Text>().text = upgrade.itemName.ToString();
                }
                else if (child.gameObject.name == "itemIcon")
                {
                    child.gameObject.GetComponent<Image>().sprite = upgrade.itemImage;
                }
            }

            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                BuyUpgrade(upgrade);
            });
        }
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        if (TeemarCount.instance.currentTeemar >= upgrade.cost)
        {
            TeemarCount.instance.currentTeemar -= upgrade.cost;
        }
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        switch(upgrade.itemName)
        {
            case "Health":
               GameManager.Instance.GetPlayer().health.AddHealth(20);
                break;
            

    
            default:
                break;
                
        }
    }

    public void ToggleShop()
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }

}

[System.Serializable]
public class Upgrade
{
    public string itemName;
    public int cost;
    public Sprite itemImage;
    [HideInInspector] public GameObject itemRef;
}
