using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Information")] //Information on the types of enemies in the game
    #region Enemy Information
    public bool bigEnemy;
    [SerializeField] private Material bigEnemyColour;


    public bool smallEnemy;
    [SerializeField] private Material smallEnemyColour;
    #endregion

    [Header("Variables")] //Variables needed for movement adn any other actions of the enemies
    [SerializeField] private bool move;
    [SerializeField] private float moveTime;


    [Header("Items")] //Items needed for the enemies to function within the play area
    [SerializeField] private GameObject target;
    private NavMeshAgent enemyAgent;
    private NavMeshSurface navSurface;

    private void Start()
    {
        //Finding some of the items needed in the hierarchy
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        navSurface = GameObject.Find("EnemyNavmesh").GetComponent<NavMeshSurface>();
        target = GameObject.Find("Tower");

        if (bigEnemy) //If a big enemy is spawned (see EnemySpawning)
        {
            gameObject.GetComponent<MeshRenderer>().material = bigEnemyColour;
            gameObject.transform.localScale = new Vector3(1.5f, gameObject.transform.localScale.y, 1.5f); // make the size of the enemy puck bigger

            //Making the bigger enemy slower by decreasing the speed and acceleration
            enemyAgent.speed = 2;
            enemyAgent.acceleration = 4;
        }
        else if (smallEnemy) //If a small enemy is spawned (see EnemySpawning)
        {
            gameObject.GetComponent<MeshRenderer>().material = smallEnemyColour;
            gameObject.transform.localScale = new Vector3(0.7f, gameObject.transform.localScale.y, 0.7f); // make the size of the enemy smaller

            //Making the smaller enemy slower by decreasing the speed and acceleration
            enemyAgent.speed = 10;
            enemyAgent.acceleration = 16;
        }
    }

    private void Update()
    {
        if (move == true) //checking if it is the enemy's turn to move
        {
            moveTime += Time.deltaTime;
            EnemyMove();
        }
        else
        {
            EnemyStop();
        }

        if (moveTime > 0.5f) // stopping movement after a certain amount of time
        {
            move = false;
            moveTime = 0f;
        }
    }

    private void EnemyMove()
    {
        navSurface.BuildNavMesh(); // Rebuilding the NavMesh in the case that there are new stationary objects on the board that the enemies need to avoid
        enemyAgent.enabled = true; // Reenabling the enemy navmesh (it is disabled when not moving in order to prevent it sliding around out of turn)

        if (enemyAgent.enabled) // Checking if the navmesh agent is enabled
        {
            enemyAgent.SetDestination(target.transform.position); // If it is enabled, move the enemy towards the tower (the target)
        }        
    }

    private void EnemyStop()
    {
        if (enemyAgent.enabled)
        {
            enemyAgent.SetDestination(transform.position); //When stopping the enemy's movement, setting it's destination to it's current destination
        }            
        enemyAgent.enabled = false; // Disabling the nav mesh agent, to prevent the enemy sliding around out of turn.
    }
}
