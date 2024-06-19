using UnityEngine;
using System;

public class AnalogClock : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    void Update()
    {
        DateTime currentTime = DateTime.Now;

        float hours = currentTime.Hour % 12;
        float minutes = currentTime.Minute;
        float seconds = currentTime.Second;

        float hourAngle = (hours + minutes / 60f) * 30f; // 360 degrees / 12 hours
        float minuteAngle = (minutes + seconds / 60f) * 6f; // 360 degrees / 60 minutes
        float secondAngle = seconds * 6f; // 360 degrees / 60 seconds

        hourHand.localRotation = Quaternion.Euler(0f, 0f, hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0f, 0f, minuteAngle);
        secondHand.localRotation = Quaternion.Euler(0f, 0f, secondAngle);
    }
}
