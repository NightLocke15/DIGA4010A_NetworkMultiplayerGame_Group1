using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using Mirror;
using UnityEngine.Serialization;
using Unity.Cinemachine;
using UnityEngine.Splines;
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
    private CharacterController controller;
    [SerializeField] private PlayerInput playerInput;
 
    [Header("Cinemachine Control")]
    [SerializeField]private CinemachineSplineDolly splineDolly;
    [SerializeField] private float dollySpeed = 5f, scrollSpeed = 10f;
    [SerializeField] private float targetDollyPos;

    //Christine Additions
    private InputSystemUIInputModule inputModule;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        targetDollyPos = splineDolly.CameraPosition;
        inputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
        gameObject.GetComponent<PlayerInput>().uiInputModule = inputModule;
        gameObject.transform.GetChild(2).GetComponent<CinemachineCamera>().Target.TrackingTarget = GameObject.Find("TargetLocation").transform;
        if (!isLocalPlayer)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (previousControlScheme != playerInput.currentControlScheme)
        {
          // Debug.Log("Hello");
            OnControlsChange(playerInput);
        }
        previousControlScheme = playerInput.currentControlScheme;

        
    }

    private void OnEnable()
    {
        realMouse = Mouse.current;
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
          //  Debug.Log(virtualMouse.name + " If");
        }
        
        else if (!virtualMouse.added)
        {
           InputSystem.AddDevice(virtualMouse);
           Debug.Log(virtualMouse.name + " else");
        }
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform == null)
        {
          Vector2 pos = cursorTransform.anchoredPosition;
          InputState.Change(virtualMouse.position, pos);
        }
        
        InputSystem.onAfterUpdate += UpdateMotion;
        playerInput.onControlsChanged += OnControlsChange;
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added)InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnControlsChange;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }
        
        //Delta
        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;
        
        Vector2 currentPos = virtualMouse.position.ReadValue();
        Vector2 newPos = currentPos + deltaValue;
        
        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height- padding);
        
        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool southbuttonTriggerIsPressed = Gamepad.current.buttonSouth.IsPressed();
        if (previousMouseState != southbuttonTriggerIsPressed)
        {
            Debug.Log("Clicked");
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, southbuttonTriggerIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = southbuttonTriggerIsPressed;
        }

        AnchorCursor(newPos);
    }

    private void AnchorCursor(Vector2 pos)
    {
        Vector2 anchorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : PlayerCamera, out anchorPos);
        cursorTransform.anchoredPosition = anchorPos;
    }

    private void OnControlsChange(PlayerInput playerInput)
    {
    // Debug.Log("Change");
        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
          //  Debug.Log("K&M");
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            realMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
            currentMouse = realMouse;
        }
        
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            //Debug.Log("Gamepad");
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, realMouse.position.ReadValue());
            AnchorCursor(realMouse.position.ReadValue());
            previousControlScheme = gamepadScheme;
            currentMouse = virtualMouse;
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed) //Checks if Click-action is pressed
        {
          // Debug.Log("Pressed");
            Physics.Raycast(PlayerCamera.ScreenPointToRay(currentMouse.position.ReadValue()), out hit); //Raycast to select objects we want to interact with
            if (hit.collider != null) // Checks if we hit something
            {
                if (hit.collider.tag == "Puck") //Checks if we hit a puck
                {
                    StartPos = currentMouse.position.ReadValue(); //Gets the current mouse position of when the button is pressed down
                    rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // Get the rigidbody of the puck
                    
                }
            }
        }

        else //Is called when the button is released
        {
         //  Debug.Log("Released");
            if (hit.collider != null) // Checks if we hit something
            {
                if (hit.collider.tag == "Puck") //Checks if we hit a puck
                {
                    Vector3 direction = new Vector3(); 
                    EndPos = currentMouse.position.ReadValue(); //Grab the position of the mouse when the button is released
                    direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).normalized; //We get only the direction between the start and end pos
                    float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).magnitude; //We get the lenght between start and end pos
                    float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
                    rb.AddForce(clampedMag * direction * mouseForce); //This shoots the puck in the direction
                    // Debug.Log("Released");
                    hit.collider.gameObject.tag = "Ally";
                    hit.collider.gameObject.layer = LayerMask.NameToLayer("Default");
                    hit = new RaycastHit(); //Reset hit
                    rb = null; //Reset rb
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
        Debug.Log(change);
        splineDolly.CameraPosition = Mathf.Lerp(splineDolly.CameraPosition, targetDollyPos,
            Time.deltaTime * dollySpeed);
    }
    // public void OnMoveMouse(InputValue value)
    // {
    //   if (value.Get<Vector2>() != Vector2.zero)
    //   {
    //       MousePos = value.Get<Vector2>();
    //   }
    // }
}
