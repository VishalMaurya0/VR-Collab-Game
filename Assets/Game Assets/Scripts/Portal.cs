using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool skipOnce = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (skipOnce)
            {
                skipOnce = false;
                return;
            }
            GameManager.Instance.timer_InsidePainting = GameManager.Instance.totalTimeInsidePainting;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable"))
        {
            GameManager.Instance.teleportObjects.Add(other.gameObject);
            //other.gameObject.SetActive(false);
        }
    }
}
