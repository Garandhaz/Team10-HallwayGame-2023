using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startingArea : MonoBehaviour
{
    public playerController playerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript.inStartingArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript.inStartingArea = false;
        }
    }
}
