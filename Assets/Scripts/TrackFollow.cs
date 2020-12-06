using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFollow : MonoBehaviour
{
    public Transform player;

    private Vector3 initialOffset;

    private void Start()
    {
        // Determine the initial spatial difference between the player object and this.
        initialOffset = player.position - transform.position;
    }

    void Update()
    {
        // Update the position (only on the Z axis, in this case) to match the initial offset.
        // This is a fake way of getting an infinite track.
        Vector3 thisOffset = player.position - initialOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y, thisOffset.z);
    }
}
