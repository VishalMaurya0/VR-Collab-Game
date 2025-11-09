using Autohand;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SoltsAndObjs : MonoBehaviour
{
    public List<PlacePoint> placePoints = new();


    private void Update()
    {
        for (int i = 0; i < placePoints.Count; i++)
        {
            PlacePoint placePoint = placePoints[i];
            if (placePoint.placedObject == null)
            {
                return;
            }
        }

        if (GameManager.Instance.unlockGate1 == false)
            UnlockGate();
    }

    private void UnlockGate()
    {
        GameManager.Instance.unlockGate1 = true;
    }
}
