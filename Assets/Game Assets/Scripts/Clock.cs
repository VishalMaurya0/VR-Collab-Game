using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public GameObject hourHand;
    public List<GameObject> gears = new();
    public List<float> gearSpeeds = new();
    public GameObject minuteHand;
    public GameObject timeManipulator;
    public bool isManipulating = false;

    public float time = 0f;

    private void Start()
    {
        for (int i = 0; i < gears.Count; i++)
        {
            gearSpeeds.Add(Random.Range(10f, 50f) * (Random.value > 0.5f ? 1 : -1));
        }
    }

    private void Update()
    {
        time += Time.deltaTime * (-1) * GameManager.Instance.timeSpeed;
        minuteHand.transform.localRotation = Quaternion.Euler(0f, 0f, -time * 6f);
        hourHand.transform.localRotation = Quaternion.Euler(0f, 0f, -time * 0.5f);

        for (int i = 0; i < gears.Count; i++)
        {
            gears[i].transform.localRotation = Quaternion.Euler(0f, 0f, -time * gearSpeeds[i]);
        }

        if (timeManipulator != null)
        {
            if (isManipulating)
                GameManager.Instance.timeSpeed = timeManipulator.transform.localEulerAngles.z / 30;
            else
                timeManipulator.transform.localEulerAngles = new Vector3(0f, 0f, GameManager.Instance.timeSpeed * 30);
        }
    }
}
