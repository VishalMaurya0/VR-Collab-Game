using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeighingMachine : MonoBehaviour
{
    public int id = 0;
    public WeighingMachineManager linkedMachine;

    public TMP_Text displayText;

    private void Start()
    {
        linkedMachine = GetComponentInParent<WeighingMachineManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        Weight weight = collision.gameObject.GetComponent<Weight>();
        if (weight != null)
        {
            linkedMachine.AddWeight(id, weight.weight);
        }
        ChangeText();
    }

    private void ChangeText()
    {
        float currentWeight = linkedMachine.code[id];
        displayText.text = currentWeight.ToString();
    }

    private void OnTriggerExit(Collider collision)
    {
        Weight weight = collision.gameObject.GetComponent<Weight>();
        if (weight != null)
        {
            linkedMachine.AddWeight(id, -weight.weight);
        }
        ChangeText();
    }

}
