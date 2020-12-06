using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StepGiver : MonoBehaviour
{
    public int stepCount = 1;

    private void OnTriggerEnter(Collider other)
    {
        // If something hits this, check for a StickManControl component.
        StickManControl stickman = other.GetComponent<StickManControl>();
        if (stickman != null)
        {
            // if it's there, hand off your steps.
            // And destroy yourself
            stickman.AddStepsToPack(stepCount);
            Destroy(gameObject);
        }
    }
}
