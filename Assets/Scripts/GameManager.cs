using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int numberOfPlayers = 1;

    // Selects number of players, and then intializes the scene with appropriate number of cars.
    public void SelectPlayerCount(int buttonValue) {
        numberOfPlayers = buttonValue;

        SceneManager.LoadScene("SkyIslands");
    }
}
