using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class guardController : MonoBehaviour
{
    public GameObject Player;
    public playerController playerScript;
    NavMeshAgent agent;

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float guardEyeLevel = 1.5f; //height of field of view
    private Vector3 fieldOfViewPosition;
    public Transform centerPoint;
    public float walkRadius;
    public float lookingTime;
    private bool looking = false;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine ("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay) //find player in Field of View at time intervals
    {
        while (true) 
        {
            yield return new WaitForSeconds (delay);
            FindVisibleTargets ();
        }
    }

    void FindVisibleTargets() 
    {
        if(agent.remainingDistance <= agent.stoppingDistance && looking == false)
        {
             StartCoroutine(lookAround(lookingTime));
        }
        fieldOfViewPosition = new Vector3(transform.position.x, guardEyeLevel, transform.position.z); //sets FoV position to custom height
        visibleTargets.Clear ();
        Collider[] targetsInViewRadius = Physics.OverlapSphere (fieldOfViewPosition , viewRadius, targetMask); //draws sphere around gameobject

        for (int i = 0; i < targetsInViewRadius.Length; i++) //For every target in the sphere (only player possible in this scenario)
        {
            Transform target = targetsInViewRadius [i].transform; //Gets target's position
            Vector3 dirToTarget = (target.position - fieldOfViewPosition).normalized; //Gets direction of target from gameobject
            if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) //If target falls within FoV angle in front of gameobject
            {
                float dstToTarget = Vector3.Distance (fieldOfViewPosition, target.position); //Gets distance between gameobject and target

                if (!Physics.Raycast (fieldOfViewPosition, dirToTarget, dstToTarget, obstacleMask) && !playerScript.isInvisible && !playerScript.inStartingArea) //If no obstacle in distance between player and gameobject
                {
                    visibleTargets.Add (target);
                    GetComponent<UnityEngine.AI.NavMeshAgent>().destination = Player.transform.position; //Moves gameobject to target's last seen position, updates when target remains in gameobject's FoV
                    looking = false;
                }
            }
        }
    }

    IEnumerator lookAround(float time)
    {
        looking = true;
        Quaternion from = transform.rotation;
        Quaternion target = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        new WaitForSeconds(1f);
        for ( float t = 0f; t < 1f; t += time * Time.deltaTime ) 
        {
            transform.rotation = Quaternion.Lerp(from,target,t);
            yield return null;
        }
        transform.rotation = target;
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            from = transform.rotation;
            target = Quaternion.Euler(0, transform.eulerAngles.y - 180, 0);
            for ( float t = 0f; t < 1f; t+= (time / 2) * Time.deltaTime ) 
            {
                transform.rotation = Quaternion.Lerp(from,target,t);
                yield return null;
            }
            transform.rotation = target;
        }
        new WaitForSeconds(1f);
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("No one found!");
            getNewPoint();
        }
    }

    public void getNewPoint()
    {
        Vector3 randomDirection = centerPoint.position + Random.insideUnitSphere * viewRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, viewRadius, 1);
        Vector3 finalPosition = hit.position;
        Debug.DrawRay(finalPosition, Vector3.up, Color.blue, 1.0f); //Debug, draws random point for visualization
        agent.SetDestination(finalPosition);
        looking = false;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        { 
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) //for use in Field of View editor script
    {
        if (!angleIsGlobal) 
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

