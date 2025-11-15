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

            GameManager.Instance.LocationSwitch(player);
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
