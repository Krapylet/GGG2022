using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public float remainingTime;

    private void Update() {
        remainingTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {

        if(other.name == "RaceCar") {
            if (remainingTime < 0) {
                Debug.Log("Jeg kom for sent :(");
            }
            else {
                Debug.Log("Jeg vandt!");
            }
        }
    }
}
