using System;
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
    [SerializeField] private RectTransform cursorTransform;
    private Mouse virtualMouse;
    private Mouse realMouse;
    public Mouse currentMouse;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private float padding = 35f;
    private bool previousMouseState;

    private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";
    
    [Header("Controller")]
    //[SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInput playerInput;
 
    [Header("Cinemachine Control")]
    [SerializeField]private CinemachineSplineDolly splineDolly;
    [SerializeField] private float dollySpeed = 5f, scrollSpeed = 10f;
    [SerializeField] private float targetDollyPos;

    [Header("Puck Control")] [SerializeField]
    private PuckScript puckScript;
    [SerializeField] private Transform placePos, storePos;
    //Christine Additions
    private InputSystemUIInputModule inputModule;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //controller = GetComponent<CharacterController>();
        targetDollyPos = splineDolly.CameraPosition;
        inputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
        gameObject.GetComponent<PlayerInput>().uiInputModule = inputModule;
        gameObject.transform.GetChild(2).GetComponent<CinemachineCamera>().Target.TrackingTarget = GameObject.Find("TargetLocation").transform;
        

        if (!isLocalPlayer)
        {

            gameObject.SetActive(false);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdAuthorityGiven(GameObject item)
    {
        if (isLocalPlayer)
        {
            item.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            Debug.Log("Authority given " + item.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    [ClientCallback]
    public void OnAttack(InputValue value)
    {
       
            if (value.isPressed) //Checks if Click-action is pressed
            {
                // Debug.Log("Pressed");
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Physics.Raycast(PlayerCamera.ScreenPointToRay(mousePos), out hit); //Raycast to select objects we want to interact with
                if (hit.collider != null) // Checks if we hit something
                {
                    if (hit.collider.tag == "Puck") //Checks if we hit a puck
                    {
                        //CmdAuthorityGiven(hit.collider.gameObject);
                        puckScript = hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck
                        GameObject puck = hit.collider.gameObject;
                        if (puckScript != null && puckScript.canDrag)
                        {
                            StartPos = Mouse.current.position.ReadValue(); //Gets the current mouse position of when the button is pressed down
                            rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // Get the rigidbody of the puck
                        }
                    }
                }
              
            }

            else //Is called when the button is released
            {
                //  Debug.Log("Released");
                if (hit.collider != null) // Checks if we hit something
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.tag == "Puck")
                    {
                        puckScript = hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck

                        if (!puckScript.isStore && puckScript.canDrag)
                        {
                            Vector3 direction = new Vector3();
                            EndPos = Mouse.current.position.ReadValue(); //Grab the position of the mouse when the button is released
                            direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).normalized; //We get only the direction between the start and end pos
                            float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).magnitude; //We get the lenght between start and end pos
                            float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
                            rb.AddForce(clampedMag * direction * mouseForce); //This shoots the puck in the direction]
                                                                              //hit.collider.gameObject.tag = "Ally";
                            hit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
                            puckScript.canDrag = false;
                            hit = new RaycastHit(); //Reset hit
                            rb.transform.parent = null;
                            rb = null; //Reset rb
                            puckScript = null; //Reset puckScript
                            Debug.Log("Fuck shit");
                        }

                        else if (puckScript.isStore)
                        {
                            Debug.Log("Test");
                            if (placePos.childCount == 0)
                            {
                                puckScript.ChangePosToBoard(placePos);
                            }
                            else if (placePos.childCount == 1)
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

    [ClientCallback]
    public void OnAttackTwo(InputValue value)
    {
        
            if (value.isPressed) //Checks if Click-action is pressed
            {
                // Debug.Log("Pressed");
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Physics.Raycast(PlayerCamera.ScreenPointToRay(mousePos), out hit); //Raycast to select objects we want to interact with
                if (hit.collider != null) // Checks if we hit something
                {
                    if (hit.collider.tag == "Puck") //Checks if we hit a puck
                    {
                       // CmdAuthorityGiven(hit.collider.gameObject);
                        puckScript = hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck
                        GameObject puck = hit.collider.gameObject;
                        if (puckScript != null && puckScript.canDrag)
                        {
                            StartPos = Mouse.current.position.ReadValue(); //Gets the current mouse position of when the button is pressed down
                            rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // Get the rigidbody of the puck
                        }
                    }
                }
            }

            else //Is called when the button is released
            {
                //  Debug.Log("Released");
                if (hit.collider != null) // Checks if we hit something
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (hit.collider.tag == "Puck")
                    {
                        puckScript = hit.collider.gameObject.GetComponent<PuckScript>(); //Grabs the puckScript on the puck

                       if (!puckScript.isStore && puckScript.canDrag)
                       {
                            Vector3 direction = new Vector3();
                            EndPos = Mouse.current.position.ReadValue(); //Grab the position of the mouse when the button is released
                            direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).normalized; //We get only the direction between the start and end pos
                            float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).magnitude; //We get the lenght between start and end pos
                            float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
                            rb.AddForce(clampedMag * direction * mouseForce); //This shoots the puck in the direction]
                                                                              //hit.collider.gameObject.tag = "Ally";
                            hit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
                            puckScript.canDrag = false;
                            hit = new RaycastHit(); //Reset hit
                            rb.transform.parent = null;
                            rb = null; //Reset rb
                            puckScript = null; //Reset puckScript
                            Debug.Log("Fuck shit");
                        }

                        else if (puckScript.isStore)
                        {
                            Debug.Log("Test");
                            if (placePos.childCount == 0)
                            {
                                puckScript.ChangePosToBoard(placePos);
                            }
                            else if (placePos.childCount == 1)
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

    public void OnLook(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        float change = dir.y * scrollSpeed;
        targetDollyPos += change;
        targetDollyPos = Mathf.Clamp(targetDollyPos, -1.5f, 1.5f);
        splineDolly.CameraPosition = Mathf.Lerp(splineDolly.CameraPosition, targetDollyPos,
            Time.deltaTime * dollySpeed);
    }

    public void OnLookTwo(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        float change = dir.y * scrollSpeed;
        targetDollyPos += change;
        targetDollyPos = Mathf.Clamp(targetDollyPos, -1.5f, 1.5f);
        splineDolly.CameraPosition = Mathf.Lerp(splineDolly.CameraPosition, targetDollyPos,
            Time.deltaTime * dollySpeed);
    }

}
