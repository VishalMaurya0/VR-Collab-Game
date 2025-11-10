using System;
using System.Collections.Generic;
using UnityEngine;

public class WeighingMachineManager : MonoBehaviour
{

    public List<int> code = new();
    public List<int> ansCode = new();

    internal void AddWeight(int id, int weight)
    {
        if (id < 0 || id >= code.Count)
        {
            Debug.LogError($"WeighingMachineManager: Invalid machine ID {id}.");
            return;
        }
        code[id] += weight;
        CheckCode();
    }

    private void CheckCode()
    {
        for (int i = 0; i < ansCode.Count; i++)
        {
            if (code[i] != ansCode[i])
                return;
        }

        GameManager.Instance.unlockGate2 = true;
    }
}
