using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        playerController controller = other.GetComponent<playerController >();
        controller.win();
    }
}
