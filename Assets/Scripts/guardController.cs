using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guardController : MonoBehaviour
{
    public GameObject Player;
    
    void Update()
    {
        GetComponent<UnityEngine.AI.NavMeshAgent>().destination = Player.transform.position;
    }
}
