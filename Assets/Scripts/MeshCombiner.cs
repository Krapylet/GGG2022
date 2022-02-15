using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel")) {
            Application.Quit();
        }
    }
}
