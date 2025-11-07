using UnityEngine;
using Autohand;

public class ButtonSetCode : MonoBehaviour
{
    public Chest targetChest;
    public int buttonIndex;    
    public int codeDigit;      
    private PhysicsGadgetButton button;

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
        targetChest.ShowButtonUI();
    }

    public void SetCodeDigit()
    {
        if (targetChest != null)
            targetChest.code[buttonIndex] = codeDigit;
        else
            Debug.LogError("Button has no targetChest assigned!");
    }
}
