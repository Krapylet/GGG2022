using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathplane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {

        // When a car hits the deathplane, respawn it back at the previous checkpoint and set it's speed to 0
        if (other.tag == "RaceCar") {
            other.transform.position = other.GetComponent<CarControls>().checkpoint.position;
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
