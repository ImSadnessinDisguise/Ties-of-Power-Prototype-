using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeemarCount : MonoBehaviour
{
    public static TeemarCount instance;

    public TMP_Text teemarText;
    public int currentTeemar = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        teemarText.text = "Teemar: " + currentTeemar.ToString();
    }

    public void IncreaseTeemar(int v)
    {
        currentTeemar += v;
        teemarText.text = "Teemar: " + currentTeemar.ToString();
    }

}
