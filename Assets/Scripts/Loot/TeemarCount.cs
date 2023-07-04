using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeemarCount : MonoBehaviour
{
    public static TeemarCount instance;

    public TMP_Text teemarText;
    public int currentTeemar = 100;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        teemarText.text = "X: " + currentTeemar.ToString();
    }

    public void IncreaseTeemar(int v)
    {
        currentTeemar += v;
        teemarText.text = "X:" + currentTeemar.ToString();
    }

}
