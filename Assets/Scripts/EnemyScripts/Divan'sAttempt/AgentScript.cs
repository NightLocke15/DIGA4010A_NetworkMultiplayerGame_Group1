using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.Serialization;
using Mirror;


public class AgentScript : NetworkBehaviour
{
    [Header("The enemy")] public ECscript connectedEnemy;
    
    [Header("NavMesh")]
    public Transform targetTransform;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LineRenderer moveLine;
    [SerializeField] private LineRenderer totalLine;
    private NavMeshPath path;
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();
    [SerializeField] private List<Vector3> pebbles = new List<Vector3>();

    
    [SerializeField] private float updateSpeed = 1000f;
    public float moveSpeed = 5f;
    
    [Header("Audio")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       agent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (connectedEnemy.turnOrderManager.currentTurn != connectedEnemy.TurnOrder) //Not enemy turn we move to the other guy very fast
        {
            SetSpeed(updateSpeed);
            MoveAgent(connectedEnemy.transform.position);
        }

    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
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

    [Command]
    private void CreateTheReachablePath(NavMeshPath thePath) //This function finds all the waypoints of where to place the linerenders points.
    {
        if (isServer)
        {
           waypoints = new List<Vector3>();
            float remainingDist = connectedEnemy.moveDistance; //How far the enemy can move

            for (int i = 0; i < thePath.corners.Length; i++)
            {
                if (i != thePath.corners.Length-1)  //if it is not the last corner
                {
                    float dist = Vector3.Distance(thePath.corners[i], thePath.corners[i + 1]); //the distance between two corners
                    
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

                else // if it is the last corner
                {
                    waypoints.Add(thePath.corners[i]);
                    SetMoveLine();
                    CreateTheTotalPath(thePath, thePath.corners[i], i);
                    return;
                }
            }
        }
    }

   // [Command]
    private void SetMoveLine()
    {
        moveLine.positionCount = waypoints.Count;
        for (int i = 0; i < waypoints.Count; i++)
        {
            moveLine.SetPosition(i, waypoints[i]);
        }
    }

   // [Command]
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

   // [Server]
    private void SetTotalLine()
    {
        totalLine.positionCount = pebbles.Count;

        for (int i = 0; i < pebbles.Count; i++)
        {
            totalLine.SetPosition(i, pebbles[i]);
        }

        if (connectedEnemy.turnOrderManager.currentTurn == connectedEnemy.TurnOrder)
        {
            SetSpeed(moveSpeed);  
            MoveAgent(waypoints.Last());
        }
    }

    public void MoveAgent(Vector3 destination)
    {
        agent.destination = destination;
    }
}
