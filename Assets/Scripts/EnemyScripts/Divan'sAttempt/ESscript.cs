using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class ESscript : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int goblinChance, orcChance, ogreChance;

    [SerializeField] private int spawnAmount;
    
    [SerializeField] private GameObject goblinPrefab, orcPrefab, ogrePrefab;

    [SerializeField] private int baseAmount = 2, adjustAmount = 2;
    [SerializeField] private TurnOrderManager turnOrderManager;
    [SerializeField] private Transform towerTransform;

    [SerializeField] private Transform enemyParent;
    
    [SyncVar]
    [SerializeField] private List<AgentScript> agentScripts;

    [SerializeField] private int increaseSpeed;

    [SerializeField] private int increaseMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Title: How to instantiate objects in a circle formation around a point?
    //Author: Cornelis-de-Jager
    //Date: 27 April 2025
    //Availability: https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980 
    //Usage: Figuring our how to calculate a point on a circle so I can spawn something there
    [Server]
    public void SummonTheEnemies()
    {
        if (isServer)
        {
            float theChance = goblinChance + orcChance + ogreChance;
            float goblinTopLimit = goblinChance;
            float orcTopLimit = goblinTopLimit + orcChance;

            spawnAmount = baseAmount + adjustAmount * turnOrderManager.totalWaves;

            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject spawnEnemy = new GameObject();
                float rad = (2 * Mathf.PI / spawnAmount * i) + 1.5708f; //Finding the degrees at which the next enemy should be spawned at

                //Finding the x and z position of the direction the enemy should be spawned in
                float xDirection = Mathf.Sin(rad); 
                float zDirection = Mathf.Cos(rad);

                Vector3 enemyDirection = new Vector3(xDirection, 1, zDirection); //The vector direction the enemy should be spawned in

                Vector3 enemyPosition = towerTransform.position + enemyDirection * 7; //The position at which the enemy is spawned based on the mid point and the radius at which it should be spawned

                float pickEnemy = Random.Range(0, theChance);

                if (0 < pickEnemy && pickEnemy <= goblinChance)
                {
                    if (goblinPrefab != null)
                    {
                        spawnEnemy = goblinPrefab;
                    }
                }
                
                else if (goblinTopLimit < pickEnemy && pickEnemy <= orcTopLimit)
                {
                    if (orcPrefab != null)
                    {
                        spawnEnemy = orcPrefab;
                    }
                }
                
                else if (orcTopLimit < pickEnemy && pickEnemy <= theChance)
                {
                    if (ogrePrefab != null)
                    {
                        spawnEnemy = ogrePrefab;
                    }
                }
                
                GameObject enemyObject  = Instantiate(spawnEnemy, enemyPosition, Quaternion.identity, enemyParent);
                NetworkServer.Spawn(enemyObject);
                agentScripts.Add(enemyObject.GetComponentInChildren<AgentScript>());
                enemyObject.GetComponentInChildren<AgentScript>().targetTransform = towerTransform;
                enemyObject.GetComponentInChildren<ECscript>().turnOrderManager = turnOrderManager;
            }

        }   
    }

   // [Server]
    public void MoveTheEnemies()
    {
        if (isServer)
        {
            int enemyCount = agentScripts.Count;

            if (enemyCount == 0)
            {
                SummonTheEnemies();
                turnOrderManager.IncreaseWaves();
            }
            
            else if (enemyCount == 1)
            {
                agentScripts[0].moveSpeed += increaseSpeed;
                agentScripts[0].connectedEnemy.moveDistance += increaseMovement;
            }
            
            else if (enemyCount > 1)
            {
                for (int i = 0; i < agentScripts.Count; i++)
                {
                    agentScripts[i].SetPath();
                }
            }
        }
    }
}
