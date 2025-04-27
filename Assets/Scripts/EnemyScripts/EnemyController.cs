using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Information")]
    #region Enemy Information
    public bool bigEnemy;
    [SerializeField] private Material bigEnemyColour;


    public bool smallEnemy;
    [SerializeField] private Material smallEnemyColour;
    #endregion

    [Header("Variables")]
    [SerializeField] private bool move;
    [SerializeField] private float moveTime;


    [Header("Items")]
    [SerializeField] private GameObject target;
    private NavMeshAgent enemyAgent;
    private NavMeshSurface navSurface;

    private void Start()
    {
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        navSurface = GameObject.Find("EnemyNavmesh").GetComponent<NavMeshSurface>();
        target = GameObject.Find("Tower");

        if (bigEnemy)
        {
            gameObject.GetComponent<MeshRenderer>().material = bigEnemyColour;
            gameObject.transform.localScale = new Vector3(1.5f, gameObject.transform.localScale.y, 1.5f);
            enemyAgent.speed = 2;
            enemyAgent.acceleration = 4;
        }
        else if (smallEnemy)
        {
            gameObject.GetComponent<MeshRenderer>().material = smallEnemyColour;
            gameObject.transform.localScale = new Vector3(0.7f, gameObject.transform.localScale.y, 0.7f);
            enemyAgent.speed = 10;
            enemyAgent.acceleration = 16;
        }
    }

    private void Update()
    {
        if (move == true)
        {
            moveTime += Time.deltaTime;
            EnemyMove();
        }
        else
        {
            EnemyStop();
        }

        if (moveTime > 0.5f)
        {
            move = false;
            moveTime = 0f;
        }
    }

    private void EnemyMove()
    {
        navSurface.BuildNavMesh();
        enemyAgent.enabled = true;
        if (enemyAgent.enabled)
        {
            enemyAgent.SetDestination(target.transform.position);
        }        
    }

    private void EnemyStop()
    {
        if (enemyAgent.enabled)
        {
            enemyAgent.SetDestination(transform.position);
        }            
        enemyAgent.enabled = false;
    }
}
