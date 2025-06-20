using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.Serialization;
using Mirror;
using Unity.VisualScripting;


public class AScript : NetworkBehaviour
{
    [Header("The enemy")] public ECscript connectedEnemy;
    
    [Header("NavMesh")]
    public Transform targetTransform;
    public NavMeshAgent agent;
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
       // agent.speed = moveSpeed;
       // NavMeshSurface navSurface = GameObject.Find("EnemyNavmesh").GetComponent<NavMeshSurface>();
       // navSurface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        // if (connectedEnemy.turnOrderManager.currentTurn != connectedEnemy.TurnOrder) //Not enemy turn we move to the other guy very fast
        // {
        //    // transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed * Time.deltaTime);
        // }
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    [Command(requiresAuthority = false)]
    public void CMDSetPath()
    {
        SetPath();
    }
    
    
    [Server]
    public void SetPath() //This checks is a path can be made to the tower
    {
        if (!isServer)
        {
            return;
        }
        transform.localPosition = connectedEnemy.transform.localPosition;
        if (agent.enabled)
        {
            path = new NavMeshPath();
            agent.CalculatePath(targetTransform.position, path);
          //  Debug.Log(path.status + " :The path status");
            switch (path.status)
            {
                case NavMeshPathStatus.PathComplete: //If the enemy can reach the tower we create the path
                   CreateTheReachablePath();
                // CmdReachablePath();
                    
                    break;
                case NavMeshPathStatus.PathPartial:
                 
                    break;
                case NavMeshPathStatus.PathInvalid:
                 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
         
    }

    [Command(requiresAuthority = false)]
    private void CmdReachablePath()
    {
        if (isServer)
        {
            CreateTheReachablePath();
        }
    }
    
    
  //  [ClientRpc]
  [Server]
    private void CreateTheReachablePath() //This function finds all the waypoints of where to place the linerenders points.
    {
        
        if (!isServer)
        {
            return;
        }
            //Debug.Log(path.status + " :The  real path status");
            
            //Debug.Log("Creating the Reachable Path");
           // Debug.Log(path.corners.Length);
            
           waypoints = new List<Vector3>();
            float remainingDist = connectedEnemy.moveDistance; //How far the enemy can move

            for (int i = 0; i < path.corners.Length; i++)
            {
                //Debug.Log(i);
                if (i == 0)
                {
                    waypoints.Add(path.corners[i]);
                }
                
                else if (i < path.corners.Length-1)  //if it is not the last corner
                {
                    //Debug.Log("i is less");
                    float dist = Vector3.Distance(path.corners[i], path.corners[i+1]); //the distance between two corners
                    //Debug.Log(dist + " remaing"+i);
                    if (remainingDist > dist)
                    {
                       // Debug.Log("Hy kan nog woema");
                        waypoints.Add(path.corners[i]);
                        remainingDist -= dist;
                    }
                    
                    else if (remainingDist <= dist)
                    {
                        //Debug.Log("Hy kan nie meer woema nie");
                        Vector3 direction = (path.corners[i] - path.corners[i+1]).normalized;
                        Vector3 pos = path.corners[i] + (direction * remainingDist);
                        waypoints.Add(pos);
                       // SetMoveLine();
                       CMDSetML(waypoints);
                        CreateTheTotalPath(pos, i);
                        return;
                    }
                }

                else // if it is the last corner
                {
                    //Debug.Log("Die laaste punt");
                    float dist = Vector3.Distance(path.corners[i-1], path.corners[i]);
                   // Debug.Log(dist + "dist");
                    if (dist > remainingDist)
                    {
                        Vector3 direction = (path.corners[i] - path.corners[i-1]).normalized;
                        Vector3 pos = path.corners[i-1] + (direction * remainingDist);
                        waypoints.Add(pos);
                        //SetMoveLine();
                        CMDSetML(waypoints);
                        CreateTheTotalPath(pos, i);
                        return;
                    }

                    else if (dist <= remainingDist)
                    {
                        waypoints.Add(path.corners[i]);
                       // SetMoveLine();
                       CMDSetML(waypoints);
                        CreateTheTotalPath( path.corners[i], i);
                        return;
                    }
                    
                }
            }
        
    }

    [Command(requiresAuthority = false)]
    private void CMDSetML(List<Vector3> waypointsL)
    {
        SetMoveLine(waypointsL);
    }

   //[Server]
   
   [ClientRpc]
    private void SetMoveLine(List<Vector3> waypointsList)
    {
        //Debug.Log("Set move line");
        //Debug.Log(waypointsList.Count + " waypoints");
        moveLine.positionCount = waypointsList.Count;
        for (int i = 0; i < waypointsList.Count; i++)
        {
            moveLine.SetPosition(i, waypointsList[i]);
        }
    }

 
    [Server]
    private void CreateTheTotalPath(Vector3 startPos, int lastIndex)
    {
        if (!isServer)
        {
            return;
        }
        
       // Debug.Log("Creating the Total Path");
        pebbles = new List<Vector3>();
        if (lastIndex != path.corners.Length)
        {
            pebbles.Add(startPos);

            for (int i = lastIndex; i < path.corners.Length; i++)
            {
                pebbles.Add(path.corners[i]); 
            }
            // SetTotalLine();
            CMDSetTL(pebbles);
        }
        
    }
 

    [Command(requiresAuthority = false)]
    private void CMDSetTL(List<Vector3> Serverpebbles)
    {
        SetTotalLine(Serverpebbles);
    }

 //  [Server]
    [ClientRpc]
    private void SetTotalLine(List<Vector3> thepebbleList)
    {
       // Debug.Log("Set total line");
       // Debug.Log(thepebbleList.Count + " pebbles");
        totalLine.positionCount = thepebbleList.Count;

        for (int i = 0; i < thepebbleList.Count; i++)
        {
            totalLine.SetPosition(i, thepebbleList[i]);
        }

        if (isServer && connectedEnemy.turnOrderManager.currentTurn == connectedEnemy.TurnOrder)
        {
            SetSpeed(moveSpeed);  
            MoveAgent(waypoints.Last());
        }
        
        else if (isServer && connectedEnemy.turnOrderManager.currentTurn != connectedEnemy.TurnOrder)
        {
            MoveAgent(connectedEnemy.transform.position);
        }
    }

 //  [Server]
    public void MoveAgent(Vector3 destination)
    {
        if (!isServer)
        {
            return;
        }
       // Debug.Log("Move agent");

       if (connectedEnemy.turnOrderManager.currentTurn == connectedEnemy.TurnOrder)
       {
           connectedEnemy.canMove = true;
           agent.destination = destination;

           if (connectedEnemy.lastEnemy)
           {
               connectedEnemy.moveDistance += connectedEnemy.increaseMoveDistance;
               connectedEnemy.lastEnemy = false;
           }
       }

       else
       {
           agent.destination = destination;
       }
        
    }

  // [Server]
    public void StopAgent()
    {
        connectedEnemy.canMove = false;
    }
}
