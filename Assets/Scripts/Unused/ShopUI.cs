using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    private Transform container;
    private Transform template;
    private IShopCustomer shopCustomer;

    private void Awake()
    {
        container = transform.Find("container");
        template = container.Find("Template");
        template.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(Item.ItemType.Weapon1, Item.GetSprite(Item.ItemType.Weapon1), "Keris", Item.GetCost(Item.ItemType.Weapon1), 0);
        CreateItemButton(Item.ItemType.Weapon2, Item.GetSprite(Item.ItemType.Weapon2), "Kerambit", Item.GetCost(Item.ItemType.Weapon2), 1);
        CreateItemButton(Item.ItemType.Weapon3, Item.GetSprite(Item.ItemType.Weapon3), "Kapak", Item.GetCost(Item.ItemType.Weapon3), 2);

        Hide();
    }

    private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite, string itemName, int itemCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(template, container);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemWidth = 130f;
        shopItemRectTransform.anchoredPosition = new Vector2(shopItemWidth * positionIndex, 0);

        shopItemTransform.Find("itemPrice").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());
        shopItemTransform.Find("itemName").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("itemIcon").GetComponent<Image>().sprite = itemSprite;

        shopItemTransform.GetComponent<ButtonUI>().ClickFunc = () => { TryBuyItem(itemType); };
    }

    public void TryBuyItem(Item.ItemType itemType)
    {
        Debug.Log("bought item" + itemType);
        shopCustomer.BoughtItem(itemType);
    }

    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

