using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guardController : MonoBehaviour
{
    public GameObject Player;

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float guardEyeLevel = 1.5f;
    private Vector3 fieldOfViewPosition;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start() 
    {
        StartCoroutine ("FindTargetsWithDelay", .2f);
    }

    void Update()
    {

    }


    IEnumerator FindTargetsWithDelay(float delay) 
    {
        while (true) {
            yield return new WaitForSeconds (delay);
            FindVisibleTargets ();
        }
    }

    void FindVisibleTargets() 
    {
        fieldOfViewPosition = new Vector3(transform.position.x, guardEyeLevel, transform.position.z);
        visibleTargets.Clear ();
        Collider[] targetsInViewRadius = Physics.OverlapSphere (fieldOfViewPosition , viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++) 
        {
            Transform target = targetsInViewRadius [i].transform;
            Vector3 dirToTarget = (target.position - fieldOfViewPosition).normalized;
            if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) 
            {
                float dstToTarget = Vector3.Distance (fieldOfViewPosition, target.position);

                if (!Physics.Raycast (fieldOfViewPosition, dirToTarget, dstToTarget, obstacleMask)) 
                {
                    visibleTargets.Add (target);
                    GetComponent<UnityEngine.AI.NavMeshAgent>().destination = Player.transform.position;
                }
            }
        }
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) 
    {
        if (!angleIsGlobal) 
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

