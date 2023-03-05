using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startingArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        playerController controller = other.GetComponent<playerController >();
        controller.inStartingArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerController controller = other.GetComponent<playerController >();
        controller.inStartingArea = false;
    }

}
