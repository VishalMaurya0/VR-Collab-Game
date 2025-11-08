using UnityEngine;
using Autohand;
using TMPro;

public class ButtonSetCode : MonoBehaviour
{
    public Chest targetChest;
    public int buttonIndex;    
    private PhysicsGadgetButton button;

    public TMP_Text text;

    void Start()
    {
        button = GetComponent<PhysicsGadgetButton>();
        if (button != null)
            button.OnPressed.AddListener(OnButtonPressed);
        else
            Debug.LogError($"No PhysicsGadgetButton found on {name}");
    }

    private void OnButtonPressed()
    {
        targetChest.ShowButtonUI(true);
        targetChest.codeIndex = buttonIndex;
    }

    //public void SetCodeDigit()
    //{
    //    if (targetChest != null)
    //        targetChest.code[buttonIndex] = codeDigit;
    //    else
    //        Debug.LogError("Button has no targetChest assigned!");
    void Update() {
        text.text = targetChest.code[buttonIndex].ToString();
    }
}
