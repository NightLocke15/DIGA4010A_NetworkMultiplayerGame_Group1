using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using Mirror;
using UnityEngine.Serialization;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Splines;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;
using UnityEngine.InputSystem.UI;
[RequireComponent(typeof(CharacterController))]


public class DragAndShoot : NetworkBehaviour
{
    [Header("Drag and release Variables")] [SerializeField]
    public float mouseForce = 20f; //Force added to the Puck for mouse drag and shoot
    [SerializeField]
    private float MaxLength = 5f; //Max distance that the mouse can pull back. The higher this is, the more force players can add by pulling further away from puck.
    private Vector2 StartPos, EndPos; //Use to get the direction in witch the player will shoot with mouse
    private Vector2 MousePos;
    private RaycastHit hit; //Selects the puck
    [SerializeField] private Rigidbody rb; //Rigidbody of the puck

    [Header("Gamepad Settings: Mouse")] //Variables used to create a virtual mouse that the gamepad can use
 
     [SerializeField] private Camera PlayerCamera;
    
    [Header("Controller")]
    //[SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInput playerInput;
 
    [Header("Cinemachine Control")]
    [SerializeField]private CinemachineSplineDolly splineDolly;
    [SerializeField] private float dollySpeed = 5f, scrollSpeed = 10f;
    [SerializeField] private float targetDollyPos;
    public Transform target;

    [Header("Puck Control")] [SerializeField]
    private PuckScript puckScript;
    [SerializeField] private Transform placePos, storePos, pucksOnBoardTransform;
    //Christine Additions
    [Header("Misc")]
    [SerializeField]
    private InputSystemUIInputModule inputModule;
    [SerializeField]
    private bool mouseDown = false; //checks if the mouse button is being held down on a puck

    [Header("Turn Order Variables")]
    [SerializeField] private int Turnorder;
    public GameObject Manager;
    [SerializeField] private TurnOrderManager turnOrderManager;
    public bool haveTakenAShot = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Manager = GameObject.Find("Manager");
        turnOrderManager = GameObject.Find("Manager").GetComponent<TurnOrderManager>();
       
        if (isLocalPlayer) //Checks if this is the playerPrefab connected to the device
        {
            Manager = GameObject.FindGameObjectWithTag("Manager");
            turnOrderManager = Manager.GetComponent<TurnOrderManager>();
            pucksOnBoardTransform = turnOrderManager.pucksOnBoard;
            playerInput.enabled = true;  //VERY IMPORTANT!!!!  Will break input system between the two players if removoved
            targetDollyPos = splineDolly.CameraPosition;
            inputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
            gameObject.GetComponent<PlayerInput>().uiInputModule = inputModule;
            if (Turnorder == 1) //Checks if this is player one
            {
                target = turnOrderManager.targetPL1;
                storePos = turnOrderManager.storeLocPl1;
                placePos = turnOrderManager.placeLocPl1;
                turnOrderManager.playerOne = this;
            }

            else if (Turnorder == 2)  //Checks if this is player two
            {
                target = turnOrderManager.targetPL2;
                storePos = turnOrderManager.storeLocPl2;
                placePos = turnOrderManager.placeLocPl2;
                turnOrderManager.playerTwo = this;
            }
            gameObject.transform.GetChild(2).GetComponent<CinemachineCamera>().Target.TrackingTarget = target;
        }

      else if (!isLocalPlayer)  //Removes clashes with other player
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            playerInput.enabled = false;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (mouseDown)
        {
            if (puckScript != null)
            {
                AimLine aimLine = puckScript.GetComponent<AimLine>();
                Vector3 direction = new Vector3();
                EndPos = Mouse.current.position
                    .ReadValue(); //Grab the position of the mouse when the button is released
                direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y)
                    .normalized; //We get only the direction between the start and end pos
                float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y)
                    .magnitude; //We get the lenght between start and end pos
                float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
               
                if (isServer) //Checks if this is the host. If not host we have to invert the direction
                {
                    aimLine.UpdateAimLine(clampedMag, direction);
                }
                else  //Inverts the direction
                {
                    aimLine.UpdateAimLine(clampedMag, -direction);
                }
            }
        }
    }

    [ClientCallback]
    public void OnAttack(InputValue value)
    {
        AimLine aimLine;
        if (turnOrderManager == null)  //Checks if we have reference to the turnOrderManager
        {
            turnOrderManager = Manager.GetComponent<TurnOrderManager>();
        }
        
        if (isLocalPlayer && Turnorder == turnOrderManager.currentTurn && !haveTakenAShot) //Checks if it is local player and if it is their turn
        {
            if (value.isPressed)  //When the button is pressed
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();  //Gets Mouse position
                Physics.Raycast(PlayerCamera.ScreenPointToRay(mousePos),
                    out hit); //Raycast to select objects we want to interact with
                if (hit.collider != null) // Checks if we hit something
                {
                    if (hit.collider.tag == "StoredPuck") //Checks if we hit a puck
                    {
                       
                        puckScript =
                            hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck
                        GameObject puck = hit.collider.gameObject;
                        aimLine = puck.GetComponent<AimLine>();
                        if (puckScript != null && puckScript.canDrag && puckScript.transform.parent == placePos)
                        {
                            StartPos = Mouse.current.position
                                .ReadValue(); //Gets the current mouse position of when the button is pressed down
                            rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // Get the rigidbody of the puck
                            aimLine.StartAimLine();
                            mouseDown = true;
                        }
                    }
                }
            }
            else //Is called when the button is released
            {
                if (hit.collider != null) // Checks if we hit something
                {
                    if (hit.collider.tag == "StoredPuck")
                    {
                        puckScript =
                            hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck


                        if (!puckScript.isStore && puckScript.canDrag && puckScript.transform.parent == placePos) //Checks if the puck is on the board and can be dragged
                        {
                            Vector3 direction = new Vector3();
                            EndPos = Mouse.current.position
                                .ReadValue(); //Grab the position of the mouse when the button is released
                            direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y)
                                .normalized; //We get only the direction between the start and end pos
                            float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y)
                                .magnitude; //We get the lenght between start and end pos
                            float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
                            if (isServer) //Checks if this is the host. If not host we have to invert the direction
                            {
                                puckScript.Drag(clampedMag, direction, mouseForce); 
                            }
                            else  //Inverts the direction
                            {
                                puckScript.Drag(clampedMag, -direction, mouseForce);
                            }
                            aimLine = puckScript.GetComponent<AimLine>();
                            aimLine.StopAimLine();
                            hit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
                            hit = new RaycastHit(); //Reset hit
                            cmdNewParent(pucksOnBoardTransform, puckScript);
                            rb = null; //Reset rb
                            puckScript = null; //Reset puckScript
                            haveTakenAShot = true;
                            turnOrderManager.WaitBeforeChangeTurn();
                            
                        }

                        else if (puckScript.isStore && puckScript.transform.parent == storePos) //checks if the puck is stored
                        {
                            if (placePos.childCount == 0) //If the puck is stored and the player has no other puck, place selcted puck on board.
                            {
                                puckScript.ChangePosToBoard(placePos);
                            }
                            else if (placePos.childCount == 1) //If there is a puck on board, remove it then add the new puck.
                            {
                                PuckScript swicthPuck = placePos.GetChild(0).GetComponent<PuckScript>();
                                swicthPuck.ChangePosToStorage(storePos);
                                puckScript.ChangePosToBoard(placePos);
                            }
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void crpcnewParent(Transform parent, PuckScript puckTrans)
    {
        puckTrans.transform.parent = parent;
        puckTrans.gameObject.tag = "Puck";
        puckTrans.canDrag = false;
    }
    

    [Command]
    public void cmdNewParent(Transform parent, PuckScript puckTrans)
    {
        if (isServer)
        {
            crpcnewParent(parent, puckTrans);
        }
        
    }
    
    
    
    public void OnLook(InputValue value)
    {
        if (isLocalPlayer)
        {
            Vector2 dir = value.Get<Vector2>(); //Checks if scroll wheel goes up or down
            float change = dir.y * scrollSpeed; //Controls how fast the camera changes position
            targetDollyPos += change;
            targetDollyPos = Mathf.Clamp(targetDollyPos, -1.5f, 1.5f); //Improves scroll function. If the values are higher/lower it influences how much the player must scroll before a change happens
            splineDolly.CameraPosition = Mathf.Lerp(splineDolly.CameraPosition, targetDollyPos, 
                Time.deltaTime * dollySpeed); //Moves the camera
        }
        
    }
    
    
    public void OnEndturn(InputValue value)
    {
        if (turnOrderManager == null) //Check if we have a reference to the TurnOrderManager
        {
             turnOrderManager = Manager.GetComponent<TurnOrderManager>();
        }

        if (isLocalPlayer && Turnorder == turnOrderManager.currentTurn) //Checks if it is the player's turn and that they are the local player.
        {
            turnOrderManager.ChangeTurn();
        }
    }


}
