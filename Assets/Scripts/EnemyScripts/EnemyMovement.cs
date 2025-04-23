using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private bool move;
    [SerializeField] private float moveTime;

    private NavMeshAgent enemyAgent;
    private NavMeshSurface navSurface;

    private void Start()
    {
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        navSurface = GameObject.Find("EnemyNavmesh").GetComponent<NavMeshSurface>();
        target = GameObject.Find("Tower");
    }

    private void Update()
    {
        if (move == true)
        {
            moveTime += Time.deltaTime;
            enemyAgent.isStopped = false;
            EnemyMove();
        }
        else
        {
            EnemyStop();
            enemyAgent.isStopped = true;
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
        enemyAgent.SetDestination(target.transform.position);
    }

    private void EnemyStop()
    {
        enemyAgent.SetDestination(transform.position);
    }
}
