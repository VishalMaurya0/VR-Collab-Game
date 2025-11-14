using Autohand;
using UnityEngine;

public class Portal : MonoBehaviour
{
    //public bool skipOnce = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            AutoHandPlayer player = other.GetComponent<AutoHandPlayer>();

            if (player != null)
            {
                // LEFT HAND
                if (player.handLeft != null && player.handLeft.GetHeld() != null)
                {
                    GameManager.Instance.helpText.text = "Release the object to enter the Portal!";
                    GameManager.Instance.timeToshowHelpText = 3f;
                    return;
                }

                // RIGHT HAND
                if (player.handRight != null && player.handRight.GetHeld() != null)
                {
                    GameManager.Instance.helpText.text = "Release the object to enter the Portal!";
                    GameManager.Instance.timeToshowHelpText = 3f;
                    return;
                }

                // Player is NOT holding anything
                GameManager.Instance.timer_InsidePainting = GameManager.Instance.totalTimeInsidePainting;
            }



        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            GameObject tele = other.gameObject;

            //if (other.transform.parent.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
            //tele = other.transform.parent.gameObject;

            if (tele.GetComponent<Grabbable>().IsHeld())
            {
                return;
            }

            GameManager.Instance.teleportObjects.Add(tele);
            //DontDestroyOnLoad(other.gameObject);
            //tele.gameObject.SetActive(false);   //TODO
            tele.GetComponent<ObjectTeleportationVisibility>().inPainting = -1;

            tele.gameObject.transform.position = GameManager.Instance.returningPos[GameManager.Instance.currentPaintingIndex];

            GameManager.Instance.timeToshowHelpText = 3f;
            GameManager.Instance.helpText.text = "Object has been teleported to the Puzzle Room!";
        }
    }
}
