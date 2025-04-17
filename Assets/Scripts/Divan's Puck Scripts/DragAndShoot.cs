using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using Mirror;
using UnityEngine.Serialization;

public class DragAndShoot : MonoBehaviour
{
    [Header("Drag and release Settings: Mouse")] [SerializeField]
    private float mouseForce = 20f; //Force added to the Puck for mouse drag and shoot
    [SerializeField]
    private float MaxLength = 5f; //Max distance that the mouse can pull back. The higher this is, the more force players can add by pulling further away from puck.
    private Vector2 StartPos, EndPos; //Use to get the direction in witch the player will shoot with mouse
    private Vector2 MousePos;
    private RaycastHit hit; //Selects the puck
    [SerializeField] private Rigidbody rb; //Rigidbody of the puck

    [Header("Gamepad Settings: Mouse")] //Variables used to create a virtual mouse that the gamepad can use
    private Mouse virtualMouse;
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private float cursorSpeed = 1000f; 
    
    private bool previousMouseState;
    
    [Header("Controller")]
    private CharacterController controller;
    [SerializeField] private PlayerInput playerInput;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice("VirtualMouse");
        }
        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform == null)
        {
          Vector2 pos = cursorTransform.anchoredPosition;
          InputState.Change(virtualMouse.position, pos);
        }
        
        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
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
        
        newPos.x = Mathf.Clamp(newPos.x, 0, Screen.width);
        newPos.y = Mathf.Clamp(newPos.y, 0, Screen.height);
        
        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool rightTriggerIsPressed = Gamepad.current.rightShoulder.IsPressed();
        if (previousMouseState != rightTriggerIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, rightTriggerIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = rightTriggerIsPressed;
        }

        AnchorCursor(newPos);
    }

    private void AnchorCursor(Vector2 pos)
    {
        Vector2 anchorPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pos, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : PlayerCamera, out anchorPos);
    }

    public void OnClick(InputValue value)
    {
        if (value.isPressed) //Checks if Click-action is pressed
        {
           Debug.Log("Click");
            Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit); //Raycast to select objects we want to interact with
            if (hit.collider != null) // Checks if we hit something
            {
                if (hit.collider.tag == "Puck") //Checks if we hit a puck
                {
                    StartPos = Mouse.current.position.ReadValue(); //Gets the current mouse position of when the button is pressed down
                    rb = hit.collider.gameObject.GetComponent<Rigidbody>(); // Get the rigidbody of the puck
                }
            }
           
            
        }

        else //Is called when the button is released
        {
            if (hit.collider != null) // Checks if we hit something
            {
                if (hit.collider.tag == "Puck") //Checks if we hit a puck
                {
                    Vector3 direction = new Vector3(); 
                    EndPos = Mouse.current.position.ReadValue(); //Grab the position of the mouse when the button is released
                    direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).normalized; //We get only the direction between the start and end pos
                    float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).magnitude; //We get the lenght between start and end pos
                    float clampedMag = Mathf.Clamp(mag, 0, MaxLength); //We put a max limit on the lenght
                    rb.AddForce(clampedMag * direction * mouseForce); //This shoots the puck in the direction
                    // Debug.Log("Released");
                    hit = new RaycastHit(); //Reset hit
                    rb = null; //Reset rb
                }
            }
        }
    }

    public void OnMoveMouse(InputValue value)
    {
      if (value.Get<Vector2>() != Vector2.zero)
      {
          MousePos = value.Get<Vector2>();
      }
    }
}
