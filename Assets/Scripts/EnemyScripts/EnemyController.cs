using Mirror;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    [Header("Enemy Information")] //Information on the types of enemies in the game
    #region Enemy Information
    public bool bigEnemy;
    [SerializeField] private Material bigEnemyColour;


    public bool smallEnemy;
    [SerializeField] private Material smallEnemyColour;
    #endregion

    [Header("Variables")] //Variables needed for movement adn any other actions of the enemies
    public bool move;
    [SerializeField] private float moveTime;
    [SerializeField] private float adjustSmall = 0.7f, adjustBig = 1.5f;


    [Header("Items")] //Items needed for the enemies to function within the play area
    [SerializeField] private GameObject target;
    private NavMeshAgent enemyAgent;
    private NavMeshSurface navSurface;
    private AudioSource audioSource;
    [SerializeField] private AudioClip moveSound;

    //[ClientRpc]
    private void Start()
    {
        //Finding some of the items needed in the hierarchy
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        navSurface = GameObject.Find("EnemyNavmesh").GetComponent<NavMeshSurface>();
        target = GameObject.Find("Tower");

        if (bigEnemy) //If a big enemy is spawned (see EnemySpawning)
        {
            gameObject.transform.localScale = new Vector3(adjustBig, gameObject.transform.localScale.y, adjustBig); // make the size of the enemy puck bigger
            gameObject.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass * adjustBig;

            //Making the bigger enemy slower by decreasing the speed and acceleration
            enemyAgent.speed = 2;
            enemyAgent.acceleration = 4;
        }
        else if (smallEnemy) //If a small enemy is spawned (see EnemySpawning)
        {
            gameObject.transform.localScale = new Vector3(adjustSmall, gameObject.transform.localScale.y, adjustSmall); // make the size of the enemy smaller
            gameObject.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass * adjustSmall;

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
            PlayMoveSound();
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

    [ClientCallback]
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

    [Command(requiresAuthority =false)]
    public void PlayMoveSound()
    {
        PlayMoveSoundRpc();
    }

    [ClientRpc]
    public void PlayMoveSoundRpc()
    {
        transform.GetComponent<AudioSource>().clip = moveSound;
        transform.GetComponent<AudioSource>().Play();
    }
}
