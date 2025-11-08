using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Chest : MonoBehaviour
{
    [Header("Chest Code")]
    public List<int> code = new();
    public int codeIndex = 0;

    [Header("UI")]
    public GameObject buttonUICanvas; 

    private List<Button> buttons = new();

    private void Start()
    {
        if (code.Count == 0)
            code.AddRange(new int[4]);

        if (buttonUICanvas != null)
            InitializeButtons();
        else
            Debug.LogWarning($"{name}: Button UI Canvas not assigned!");
    }

    private void InitializeButtons()
    {
        buttons.Clear();
        Button[] foundButtons = buttonUICanvas.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < foundButtons.Length; i++)
        {
            int index = i;
            Button btn = foundButtons[i];
            buttons.Add(btn);

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnButtonPressed(index));


            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = $"{index + 1}";
        }

        Debug.Log($"{name}: Found and linked {buttons.Count} buttons.");
    }

    private void OnButtonPressed(int buttonIndex)
    {
        code[codeIndex] = (buttonIndex + 1);


        codeIndex++;
        if (codeIndex >= 4)
        {
            ShowButtonUI(false);
        }

            codeIndex %= 4;
    }

    public void ShowButtonUI(bool show)
    {
        if (buttonUICanvas != null)
            buttonUICanvas.SetActive(show);
    }
}
