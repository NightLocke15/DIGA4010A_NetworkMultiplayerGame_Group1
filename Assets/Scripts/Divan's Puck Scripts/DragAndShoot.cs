using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
    
    [Header("Controller")]
    private CharacterController controller;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(InputValue value)
    {
        if (value.isPressed) //Checks if Click-action is pressed
        {
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
          Debug.Log(MousePos);
      }
    }
}
