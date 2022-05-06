using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyIslandsManager : MonoBehaviour
{
    //Bilernes prefabs
    public GameObject soloCar; // For singleplayer
    public GameObject player1Car; // for multiplayer
    public GameObject player2Car;

    //Steder hvor bilerne kan spawne
    public Transform spawnPointLeft;
    public Transform spawnPointRight;


    // Start is called before the first frame update
    void Start()
    {
        int noOfPlayers = GameManager.numberOfPlayers;

        if (noOfPlayers == 1) {
            SpawnCar(soloCar, spawnPointLeft);
        }
        else if (noOfPlayers == 2) {
            SpawnCar(player1Car, spawnPointLeft);
            SpawnCar(player2Car, spawnPointRight);
        }
    }


    private void SpawnCar(GameObject carPrefab, Transform SpawnPosition) {
        // Find ud af hvor og hvordan bilen skal stå
        Vector3 spawnPos = SpawnPosition.position;
        Quaternion spawnRotation = SpawnPosition.rotation;

        // Skab bilen
        Instantiate(carPrefab, spawnPos, spawnRotation);
    }

}
