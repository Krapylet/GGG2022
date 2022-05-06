using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {

        if(other.tag == "RaceCar") {
            // Get UI acess
            CarUI ui = other.GetComponent<CarUI>();

            // Increase the lap counter by one.
            ui.lapCounter++;

            //Update the UI text. becomes "Runde x/3"
            ui.lapCounterText.text = "Runde: " + ui.lapCounter + "/3";

            if (ui.lapCounter == 4) {
                // Spillet er færdig. Gå til menu.
                SceneManager.LoadScene("StartMenu");
            }
        }
    }
}
