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
            GameManager.Instance.timeOfStayText.gameObject.SetActive(true);
            GameManager.Instance.timeOfStayText.text = "Press Right Hand Trigger Button to Go inside The Painting!!";

            GameManager.Instance.teleportable = true;
            GameManager.Instance.toSetPaintingIndex = paintingID;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.timeOfStayText.gameObject.SetActive(false);
            GameManager.Instance.teleportable = false;
        }
    }
}
