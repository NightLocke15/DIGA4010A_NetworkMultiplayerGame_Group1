using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.Serialization;
using Mirror;


public class AgentScript : NetworkBehaviour
{
    [Header("The enemy")] [SerializeField] private ECscript connectedEnemy;
    
    [Header("NavMesh")]
    public Transform targetTransform;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LineRenderer moveLine;
    [SerializeField] private LineRenderer totalLine;
    [SerializeField] private NavMeshPath path;
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();
    [SerializeField] private List<Vector3> pebbles = new List<Vector3>();
    
    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SetPath() //This checks is a path can be made to the tower
    {
         path = new NavMeshPath();
         agent.CalculatePath(targetTransform.position, path);

         switch (path.status)
         {
             case NavMeshPathStatus.PathComplete: //If the enemy can reach the tower we create the path
                 CreateTheReachablePath(path);
                 break;
             case NavMeshPathStatus.PathPartial:
                 
                 break;
             case NavMeshPathStatus.PathInvalid:
                 
                 break;
             default:
                 throw new ArgumentOutOfRangeException();
         }
    }

    [Server]
    private void CreateTheReachablePath(NavMeshPath thePath) //This function finds all the waypoints of where to place the linerenders points.
    {
        if (isServer)
        {
           waypoints = new List<Vector3>();
            float remainingDist = connectedEnemy.moveDistance;

            for (int i = 0; i < thePath.corners.Length; i++)
            {
                if (i != thePath.corners.Length-1)
                {
                    float dist = Vector3.Distance(thePath.corners[i], thePath.corners[i + 1]);
                    
                    if (remainingDist > dist)
                    {
                        waypoints.Add(thePath.corners[i]);
                        remainingDist -= dist;
                    }
                    
                    else if (remainingDist <= dist)
                    {
                        Vector3 direction = (thePath.corners[i] - thePath.corners[i + 1]).normalized;
                        Vector3 pos = thePath.corners[i] + (direction * remainingDist);
                        waypoints.Add(pos);
                        SetMoveLine();
                        CreateTheTotalPath(thePath, pos, i);
                        return;
                    }
                }

                else
                {
                    waypoints.Add(thePath.corners[i]);
                    SetMoveLine();
                    return;
                }
            }
        }
    }

    [Server]
    private void SetMoveLine()
    {
        moveLine.positionCount = waypoints.Count;
        for (int i = 0; i < waypoints.Count; i++)
        {
            moveLine.SetPosition(i, waypoints[i]);
        }
    }

    [Server]
    private void CreateTheTotalPath(NavMeshPath thePath, Vector3 startPos, int lastIndex)
    {
        if (isServer)
        {
            pebbles = new List<Vector3>();
            if (lastIndex != thePath.corners.Length - 1)
            {
                pebbles.Add(startPos);

                for (int i = lastIndex; i < thePath.corners.Length; i++)
                {
                    pebbles.Add(thePath.corners[i]); 
                }
                SetTotalLine();
            }
        }
    }

    [Server]
    private void SetTotalLine()
    {
        totalLine.positionCount = pebbles.Count;

        for (int i = 0; i < pebbles.Count; i++)
        {
            totalLine.SetPosition(i, pebbles[i]);
        }
    }

    public void MoveAgent(Vector3 destination)
    {
        agent.destination = destination;
    }
}
