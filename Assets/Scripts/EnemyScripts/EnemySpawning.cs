using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;
using Mirror;

public class EnemySpawning : NetworkBehaviour
{
    [Header("Variables")] //Variables needed to keep track of the enemies spawned
    public bool spawned;

    public int wave = 1;
    [SerializeField] private int maxSpawn;
    public int spawnCount = 0;
    public int bigSpawn = 0;
    public int smallSpawn = 0;
    [SyncVar]
    private bool started;

    private float waitTime = 0.2f;

    [SyncVar]
    public List<EnemyController> spawnList = new List<EnemyController>();

    [Header("Items")] //Items needed to spawn enemies
    [SerializeField] private GameObject bigEnemy; //Enemy prefab to be instantiated
    [SerializeField] private GameObject smallEnemy; //Enemy prefab to be instantiated
    [SerializeField] private GameObject tower; //Tower to determine the mid point of the circle around which the enemies should be spawned
    [SerializeField] private MenuUI menuScript;
    [SerializeField] private GameObject Manager;

    private void Start()
    {
        tower = GameObject.Find("Tower");
        //if (isServer)
        //{
        //    SpawnEnemies();
        //}
        
    }

    // public void CallSpawnEnemies()
    // {
    //     if (isServer)
    //     {
    //         SpawnEnemies();
    //     }
    // }

    
    private void Update()
    {
        
        

        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < spawnList.Count; i++) 
            {
                spawnList[i].move = true;
            }
        }
    }


    
    //Title: How to instantiate objects in a circle formation around a point?
    //Author: Cornelis-de-Jager
    //Date: 27 April 2025
    //Availability: https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980 
    //Usage: Figuring our how to calculate a point on a circle so I can spawn something there
    [Server]
    public void SpawnEnemies()
    {
        if (isServer)
        {
            maxSpawn = 2 + 2 * wave; //How many enemies should be spawned based on the current wave
        for (int i = 0; i < maxSpawn; i++)
        {
            float rad = (2 * Mathf.PI / maxSpawn * i) + 1.5708f; //Finding the degrees at which the next enemy should be spawned at

            //Finding the x and z position of the direction the enemy should be spawned in
            float xDirection = Mathf.Sin(rad); 
            float zDirection = Mathf.Cos(rad);

            Vector3 enemyDirection = new Vector3(xDirection, 1, zDirection); //The vector direction the enemy should be spawned in

            Vector3 enemyPosition = tower.transform.position + enemyDirection * 7; //The position at which the enemy is spawned based on the mid point and the radius at which it should be spawned

            //Spawing a big enemy and then a small enemy
            if (bigSpawn <= smallSpawn) 
            {
                GameObject enemyObject = Instantiate(bigEnemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false; //Disabling the NavMeshAgent in order to prevent the enemy sliding around out of turn
                enemyObject.GetComponent<EnemyController>().bigEnemy = true; //Determining what type of enemy it will be
                enemyObject.GetComponent<AudioSource>().enabled = false;
                NetworkServer.Spawn(enemyObject);
                bigSpawn++;
                spawnList.Add(enemyObject.GetComponent<EnemyController>());
                enemyObject.GetComponent<EnemyController>().SpawnedIn(0);
            }
            else if (smallSpawn < bigSpawn)
            {
                GameObject enemyObject = Instantiate(smallEnemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false; //Disabling the NavMeshAgent in order to prevent the enemy sliding around out of turn
                enemyObject.GetComponent<EnemyController>().smallEnemy = true; //Determining what type of enemy it will be
                enemyObject.GetComponent<AudioSource>().enabled = false;
                NetworkServer.Spawn(enemyObject);
                smallSpawn++;
                spawnList.Add(enemyObject.GetComponent<EnemyController>());
                enemyObject.GetComponent<EnemyController>().SpawnedIn(1);
            }
           // yield return new WaitForSeconds(wait); //Wait a small amount of time before spawing the next enemy
        }
        }
        
    }

    public void RemoveTheDead()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            if (spawnList[i] == null)
            {
                spawnList.RemoveAt(i);
            }
        }


    }

    public void MoveEnemies()
    {
        RemoveTheDead();

        if (started)
        {
            if (spawnList.Count == 0)
            {
                if (isServer)
                {
                    wave++;
                    SpawnEnemies();
                }

            }
        }

        for (int i = 0; i < spawnList.Count; i++)
        {
            if (spawnList[i] != null)
            {
                spawnList[i].move = true;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void StartGame()
    {
        StartGameFunctions();
        SpawnEnemies();
        started = true;
    }

    [ClientRpc]
    public void StartGameFunctions()
    {
        menuScript.waitingPanel.SetActive(false);
        if (isServer)
        {
            //SpawnEnemies();
            StartCoroutine(StartFirstTurn());
        }
    }

    //https://discussions.unity.com/t/how-to-wait-before-running-a-function-called-in-start/631729/2
    private IEnumerator StartFirstTurn()
    {
        yield return new WaitForSeconds(2f);
        Manager.GetComponent<TurnOrderManager>().FirstTurn();
    }
}
