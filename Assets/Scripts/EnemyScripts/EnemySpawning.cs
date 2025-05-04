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

    private float waitTime = 0.2f;

    [SerializeField] private List<GameObject> spawnList = new List<GameObject>();

    [Header("Items")] //Items needed to spawn enemies
    [SerializeField] private GameObject enemy; //Enemy prefab to be instantiated
    [SerializeField] private GameObject tower; //Tower to determine the mid point of the circle around which the enemies should be spawned

    private void Start()
    {
        tower = GameObject.Find("Tower");
        if (isServer)
        {
            SpawnEnemies();
        }
        
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            for (int i = 0; i < spawnList.Count; i++) 
            {
                spawnList[i].GetComponent<EnemyController>().move = true;
            }
        }
    }


    //https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980
    private void SpawnEnemies()
    {
        maxSpawn = 4 + 2 * wave; //How many enemies should be spawned based on the current wave
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
                GameObject enemyObject = Instantiate(enemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false; //Disabling the NavMeshAgent in order to prevent the enemy sliding around out of turn
                enemyObject.GetComponent<EnemyController>().bigEnemy = true; //Determining what type of enemy it will be
                NetworkServer.Spawn(enemyObject);
                bigSpawn++;
                spawnList.Add(enemyObject);
            }
            else if (smallSpawn < bigSpawn)
            {
                GameObject enemyObject = Instantiate(enemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false; //Disabling the NavMeshAgent in order to prevent the enemy sliding around out of turn
                enemyObject.GetComponent<EnemyController>().smallEnemy = true; //Determining what type of enemy it will be
                NetworkServer.Spawn(enemyObject);
                smallSpawn++;
                spawnList.Add(enemyObject);
            }
           // yield return new WaitForSeconds(wait); //Wait a small amount of time before spawing the next enemy
        }
    }
}
