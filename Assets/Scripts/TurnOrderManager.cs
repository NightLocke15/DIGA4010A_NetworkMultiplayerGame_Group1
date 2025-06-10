using UnityEngine;
using Mirror;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.SceneManagement;

public class TurnOrderManager : NetworkBehaviour
{
    [SerializeField] [SyncVar] public int currentTurn = -1;
    [SyncVar] public int totalWaves = 0;
    [SerializeField] private EnemySpawning enemySpawning;
    [SerializeField] private GameObject Manager;
    [SerializeField] private GameObject Tower;
    [FormerlySerializedAs("EnemiesAreMoving")] [SerializeField] private bool enemiesAreMoving;
    [SerializeField] private bool shouldChangeOrder = false;
    [SerializeField] private float moveTime = 0f, waitTime = 0f, bufferTime = 2f;
    [FormerlySerializedAs("storeLocPL1")] [Header("Player One Info")] [SerializeField]
    public Transform storeLocPl1;

     [Header("Player One Info")] [SerializeField]
    public Transform placeLocPl1;
    [SyncVar]
    public string playerOneText;

   
    [Header("Player Two Info")]
    [SerializeField] public Transform storeLocPl2;
    [SyncVar]
    public string playerTwoText;


    [Header("Player Two Info")]
    [SerializeField] public Transform placeLocPl2;

   
   [Header("Important Info")] public Transform pucksOnBoard;
   //[SerializeField] private PuckScript pl1BP, pl2BP, pl1SP, pl2SP;
   [SerializeField] private GameObject PuckPrefab;
   [SerializeField] private int PucksAmountStorage = 1;
   public DragAndShoot playerOne, playerTwo;


    public GameObject endScreen;
    public GameObject canvasObject;
   
   [Header("Camera TagetPos")]
   public Transform targetPL1, targetPL2;
   
   [SerializeField]
   private ESscript escript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemySpawning = gameObject.GetComponent<EnemySpawning>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldChangeOrder)
        {
            waitTime += Time.deltaTime;
        }

        if (waitTime > bufferTime)
        {
            shouldChangeOrder = false;
            waitTime = 0;
            ChangeTurn();
            escript.StopMovement();
            if (isServer)
            {
                playerOne.haveTakenAShot = false;
            }
            else
            {
                playerTwo.haveTakenAShot = false;
            }
            
        }
        
        if (enemiesAreMoving) //Checks when enemies are moving
        {
            moveTime += Time.deltaTime; //Buffer time. Gives enemies time to move. 
        }

        if (moveTime > bufferTime)  //How long the buffer time is
        {
            
            enemiesAreMoving = false; //Stops counter
            ChangeTurn();  //enmies are moving
            escript.StopMovement();
            moveTime = 0f; //Resets timer
        }

        if (currentTurn == 0)
        {
            playerOneText = "Enemy Turn";
            playerTwoText = "Enemy Turn";
        }
        
        else if (currentTurn == 1)
        {
            playerOneText = "Your Turn!";
            playerTwoText = "Player One Turn";
        }
        
        else if (currentTurn == 2)
        {
            playerOneText = "Player Two Turn.";
            playerTwoText = "Your Turn!";
        }
    }

    [Command(requiresAuthority = false)]
    public void ChangeTurn()  //Changes the turnOrder
    {
        if (isServer)
        {
            int placeCountPl1 = placeLocPl1.childCount;
            int placeCountPl2 = placeLocPl2.childCount;
            int storeCountPl1 = storeLocPl1.childCount;
            int storeCountPl2 = storeLocPl2.childCount;

            if ((placeCountPl1 == 0 && storeCountPl1 == 0) && (placeCountPl2 == 0 && storeCountPl2 == 0) || Tower.GetComponent<TowerHandler>().towerHealth <= 0)
            {
                //Defeat function
                
                CmdEndScreen();
            }

            else
            {
                currentTurn++;   // Changes the turn order
                //Debug.Log(isServer);
                
                if (currentTurn > 2)  //If turnorder is more than two we reset it
                {
                    currentTurn = 0;
            
                }
                
                escript.MoveTheEnemies();
        
                if (currentTurn == 0)
                {
                    
                    //enemySpawning.MoveEnemies(); //Moves the enemies
                    enemiesAreMoving = true;     //Starts timer
                    return;
                }
        
                else if (currentTurn == 1)
                {
                    if (placeCountPl1 == 0 && storeCountPl1 == 0)
                    {
                        WaitBeforeChangeTurn();
                        return;
                    }
         
                }
        
                else if (currentTurn == 2)
                {
                    if (placeCountPl2 == 0 && storeCountPl2 == 0)
                    {
                        WaitBeforeChangeTurn();
                        return;
                    }
            
                }
            }
        }
    }

    public void WaitBeforeChangeTurn() //A buffer before the turn order is changed, so that pucks can finish moving before the turn ends
    {
        shouldChangeOrder = true;
    }

    public void FirstTurn()
    {
        if (isServer)
        {
          WaitBeforeChangeTurn();
        }
    }

    // [Command]
    // public void cmdAssignParents(int value)
    // {
    //     AssignParents(value);
    // }

    [Server]
    public void AssignParents(int numPlayers)
    {
        if (isServer)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                if (i == 0)
                {
                    GameObject boardPuck = Instantiate(PuckPrefab,placeLocPl1.position, Quaternion.identity);
                    NetworkServer.Spawn(boardPuck);
                    boardPuck.GetComponent<PuckScript>().ChangePosToBoard(placeLocPl1);
                    for (int j = 0; j < PucksAmountStorage; j++)
                    {
                        Vector3 pos = new Vector3(storeLocPl1.position.x, storeLocPl1.position.y + 2, storeLocPl1.position.z);
                        GameObject storagePuck = Instantiate(PuckPrefab, pos, Quaternion.identity);
                        NetworkServer.Spawn(storagePuck);
                        storagePuck.GetComponent<PuckScript>().ChangePosToStorage(storeLocPl1);
                    }
                }
                else if (i == 1)
                {
                    GameObject boardPuck = Instantiate(PuckPrefab,placeLocPl2.position, Quaternion.identity);
                    NetworkServer.Spawn(boardPuck);
                    boardPuck.GetComponent<PuckScript>().ChangePosToBoard(placeLocPl2);
                    for (int j = 0; j < PucksAmountStorage; j++)
                    {
                        Vector3 pos = new Vector3(storeLocPl2.position.x, storeLocPl2.position.y + 2, storeLocPl2.position.z);
                        GameObject storagePuck = Instantiate(PuckPrefab,pos, Quaternion.identity);
                        NetworkServer.Spawn(storagePuck);
                        storagePuck.GetComponent<PuckScript>().ChangePosToStorage(storeLocPl2);
                    }
                }
            }
        }
       
    }

    [Command(requiresAuthority = false)] 
    public void CmdEndScreen()
    {
        EndScreen();
    }

    [ClientRpc] 
    public void EndScreen()
    {
        Debug.Log("And Yes");
      //  Manager.GetComponent<EnemySpawning>().enabled = false;
        endScreen.SetActive(true);
    }

    [Command(requiresAuthority = false)]
    public void IncreaseWaves()
    {
        if (isServer)
        {
            totalWaves++;
        }
    }
}
