using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.timer_InsidePainting = GameManager.Instance.totalTimeInsidePainting;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            GameManager.Instance.teleportObjects.Add(other.gameObject);
            //other.gameObject.SetActive(false);
        }
    }
}
