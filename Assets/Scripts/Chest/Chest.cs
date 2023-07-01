using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IUseable
{
    #region Tooltip
    [Tooltip("Set this colour to be used for the materialization effect")]
    #endregion
    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;

    #region Tooltip
    [Tooltip("Set this to the time it will take to materialize chest")]
    #endregion
    [SerializeField] private float materializeTime = 3f;

    #region Tooltip
    [Tooltip("Populate with item spawn point")]
    #endregion
    [SerializeField] private Transform itemSpawnPoint;

    private int healthPercent;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        materializeEffect = GetComponent<MaterializeEffect>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(bool shouldMaterialize, int healthPercent)
    {
        this.healthPercent = healthPercent;

        if (shouldMaterialize)
        {
            StartCoroutine(MaterializeChest());
        }
        else
        {
            EnableChest();
        }
    }

    private IEnumerator MaterializeChest()
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.instance.materializeShader,materializeColor, materializeTime,
            spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();

    }

    private void EnableChest()
    {
        isEnabled = true;
    }

    public void UseItem()
    {
        if (!isEnabled) return;

        switch (chestState)
        {
            case ChestState.closed:
                OpenChest();
                break;

            case ChestState.healthItem:
                CollectHealthItem();
                break;

            case ChestState.empty:
                return;


            default:
                return;

        }

    }

    private void OpenChest()
    {
        animator.SetBool("OpenChest", true);

        UpdateChestState();
    }

    private void UpdateChestState()
    {
        if (healthPercent != 0)
        {
            InstantiateHealthItem();
        }
        else
        {
            chestState = ChestState.empty;
        }
    }

    private void InstantiateItem()
    {
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);
        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    private void InstantiateHealthItem()
    {
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIcon, itemSpawnPoint.position, materializeColor);
    }

    private void CollectHealthItem()
    {
        if (chestItem == null || chestItem.isItemMaterialized) return;

        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

        healthPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }
}
