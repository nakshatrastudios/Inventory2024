using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform targetParent;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    void Start()
    {
        waypoints = targetParent.GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        if (currentWaypoint < waypoints.Length)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, Time.deltaTime);

            if (transform.position == waypoints[currentWaypoint].position)
            {
                currentWaypoint++;
            }

            if (currentWaypoint == waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }
    }
}
