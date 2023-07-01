using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    
    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void Initialize(Sprite sprite, Vector3 spawnPosition, Color materializeColor)
    {
        spriteRenderer.sprite = sprite;
        transform.position = spawnPosition;

        StartCoroutine(MaterializeItem(materializeColor));
    }

    private IEnumerator MaterializeItem(Color materializeColor)
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader, materializeColor, 1f, spriteRendererArray,
            GameResources.Instance.litMaterial));

        isItemMaterialized = true;
    }
}
