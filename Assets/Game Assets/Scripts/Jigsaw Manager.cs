using Autohand;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class JigsawManager : MonoBehaviour
{
    public List<PlacePoint> placePoints;
    public bool puzzleCompleted = false;
    public bool openDoor = false;
    public GameObject lockerGate;
    public GameObject finalLetter;
    public Animator paintingAnimator;

    public List<PlacePoint> cups = new ();

    private void Update()
    {
        if (!puzzleCompleted)
            CheckCompletion();

        if (puzzleCompleted && !openDoor)
        {
            lockerGate.GetComponent<Rigidbody>().isKinematic = false;
            paintingAnimator.SetBool("isUnlocked", true);

            for (int i = 0; i < cups.Count; i++)
            {
                PlacePoint cupPoint = cups[i];
                if (cupPoint.placedObject == null)
                {
                    return;
                }
            }

            openDoor = true;
            GameManager.Instance.unlockGate3 = true;
            finalLetter.SetActive(true);

            //drop sound
        }
    }

    public void CheckCompletion()
    {
        foreach (PlacePoint point in placePoints)
        {
            if (point.placedObject == null)
                return;

            string name = point.placedObject.name;
            string expectedName = point.gameObject.name;

            Match pointMatch = Regex.Match(expectedName, @"\d+$");
            Match objMatch = Regex.Match(name, @"\d+$");

            if (pointMatch.Success && objMatch.Success)
            {
                string pointNum = pointMatch.Value;
                string objNum = objMatch.Value;

                if (pointNum == objNum)
                    Debug.Log(" Numbers match! Object placed correctly.");
                else
                {
                    Debug.Log($"Mismatch: Point {pointNum}, Object {objNum}");
                    return;
                }
            }
            else
            {
                Debug.LogWarning(" One or both names don't contain a number at the end.");
                return;
            }
        }

        puzzleCompleted = true;
    }
}
