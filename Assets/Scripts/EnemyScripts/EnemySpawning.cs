using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] private bool spawned;
    [SerializeField] private int spawnCount = 0;
    [SerializeField] private GameObject enemy;

    private float waitTime = 0.2f;

    [SerializeField] private List<Vector3> positions = new List<Vector3>();
    [SerializeField] private List<bool> positionTrue = new List<bool>();

    private void Update()
    {
        if (spawned == false)
        {
            StartCoroutine(SpawnEnemies(waitTime));
        }

        if (spawnCount >= 6)
        {
            spawned = true;
        }
    }

    private IEnumerator SpawnEnemies(float wait)
    {
        for (int i = 0; i < positions.Count; i++) { 
            if (positionTrue[i] == false)
            {
                GameObject Object = Instantiate(enemy, positions[i], Quaternion.identity);
                spawnCount++;
                positionTrue[i] = true;
                
            }
            yield return new WaitForSeconds(wait);
        }
    }
}
