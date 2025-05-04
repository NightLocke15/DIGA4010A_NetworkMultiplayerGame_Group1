using UnityEngine;
using Mirror;
using UnityEngine.Serialization;

public class TurnOrderManager : NetworkBehaviour
{
    [SerializeField] [SyncVar] public int currentTurn = -1;
    [SerializeField] private EnemySpawning enemySpawning;
    [SerializeField] private bool EnemiesAreMoving;
    [SerializeField] private float moveTime = 0f;

    [Header("Player One Info")] [SerializeField]
    public Transform StoreLocPL1, PlaceLocPL1;
    
    [Header("Player Two Info")]
    [SerializeField] public Transform StoreLocPL2, PlaceLocPL2;

    [Header("Important Info")] public Transform PucksOnBoard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemySpawning = gameObject.GetComponent<EnemySpawning>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (EnemiesAreMoving) //Checks when enemies are moving
        {
            moveTime += Time.deltaTime; //Buffer time. Gives enemies time to move. 
        }

        if (moveTime >= 1f)  //How long the buffer time is
        {
            EnemiesAreMoving = false; //Stops counter
            ChangeTurn();  //enmies are moving
            moveTime = 0f; //Resets timer
        }

        if (currentTurn == 0)
        {
            //enemies turn
        }
        
        else if (currentTurn == 1)
        {
            //PL 1 turn
        }
        
        else if (currentTurn == 2)
        {
            //PL 2 turn
        }
    }
    
    [Command(requiresAuthority = false)]
    public void ChangeTurn()  //Changes the turnOrder
    {
        currentTurn++;   // Changes the turn order
        if (currentTurn > 2)  //If turnorder is more than two we reset it
        {
            currentTurn = 0;
            
        }
        
        if (currentTurn == 0)
        {
            enemySpawning.MoveEnemies(); //Moves the enemies
            EnemiesAreMoving = true;     //Starts timer
        }
    }

    public void FirstTurn()
    {
        if (isServer)
        {
            ChangeTurn();
        }
    }
}
