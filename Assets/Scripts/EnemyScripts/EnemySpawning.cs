using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class EnemySpawning : MonoBehaviour
{
    [Header("Variables")]
    public bool spawned;

    public int wave = 1;
    [SerializeField] private int maxSpawn;
    public int spawnCount = 0;
    public int bigSpawn = 0;
    public int smallSpawn = 0;

    private float waitTime = 0.2f;

    [Header("Items")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject tower;

    private void Start()
    {
        tower = GameObject.Find("Tower");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(SpawnEnemies(waitTime));
        }
    }


    //https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980
    private IEnumerator SpawnEnemies(float wait)
    {
        maxSpawn = 4 + 2 * wave;
        for (int i = 0; i < maxSpawn; i++)
        {
            float rad = (2 * Mathf.PI / maxSpawn * i) + 1.5708f;

            float xPos = Mathf.Sin(rad);
            float zPos = Mathf.Cos(rad);

            Vector3 enemyDirection = new Vector3(xPos, 10, zPos);

            Vector3 enemyPosition = tower.transform.position + enemyDirection * 7;

            if (bigSpawn <= smallSpawn)
            {
                GameObject enemyObject = Instantiate(enemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false;
                enemyObject.GetComponent<EnemyController>().bigEnemy = true;
                bigSpawn++;
            }
            else if (smallSpawn < bigSpawn)
            {
                GameObject enemyObject = Instantiate(enemy, enemyPosition, Quaternion.identity);
                enemyObject.GetComponent<NavMeshAgent>().enabled = false;
                enemyObject.GetComponent<EnemyController>().smallEnemy = true;
                smallSpawn++;
            }
            yield return new WaitForSeconds(wait);
        }


        //for (int i = 0; i < positions.Count; i++) {
        //    if (positionTrue[i].gameObject == null)
        //    {
        //        int randomNum = Random.Range(1, 2);

        //        if (randomNum == 1 && bigSpawn < 3)
        //        {
        //            GameObject enemyObject = Instantiate(enemy, positions[i], Quaternion.identity);
        //            enemyObject.GetComponent<NavMeshAgent>().enabled = false;
        //            enemyObject.GetComponent<EnemyController>().bigEnemy = true;
        //            spawnCount++;
        //            bigSpawn++;
        //            positionTrue[i] = enemyObject;
        //        }
        //        else if (randomNum == 2 && smallSpawn < 3)
        //        {
        //            GameObject enemyObject = Instantiate(enemy, positions[i], Quaternion.identity);
        //            enemyObject.GetComponent<NavMeshAgent>().enabled = false;
        //            enemyObject.GetComponent<EnemyController>().smallEnemy = true;
        //            spawnCount++;
        //            smallSpawn++;
        //            positionTrue[i] = enemyObject;
        //        }               
        //    }
        //    yield return new WaitForSeconds(wait);
        //}
    }
}
