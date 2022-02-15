using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterScript : MonoBehaviour
{
    public float boostStrength;

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "RaceCar") {
            Debug.Log("booost!");
            Vector3 boostForce = gameObject.transform.forward * boostStrength;
            other.GetComponent<Rigidbody>().AddForce(boostForce);
        }
    }
}
