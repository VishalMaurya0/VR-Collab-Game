using TMPro;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public int paintingID;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Player is inside painting trigger.");
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.timeOfStayText.SetActive(true);
            TMP_Text text = GameManager.Instance.timeOfStayText.GetComponent<TMP_Text>();
            text.text = "Press Right Hand Trigger Button to Go inside The Painting!!";

            GameManager.Instance.teleportable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.timeOfStayText.SetActive(false);
            GameManager.Instance.teleportable = false;
        }
    }
}
