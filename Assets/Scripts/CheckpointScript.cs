using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {

        // When a car hits this checkpoint, save this position as the respawn position.
        if (other.tag == "RaceCar") {
            other.GetComponent<CarControls>().checkpoint = transform;
        }
    }
}
