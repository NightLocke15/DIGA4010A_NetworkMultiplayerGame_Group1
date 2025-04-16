using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class DragAndShoot : MonoBehaviour
{
    [Header("Drag and release Settings")] [SerializeField]
    private float Force = 20f;
    [SerializeField]
    private float MaxLength = 5f;
    private Vector2 StartPos, EndPos;
    private Vector2 MousePos;
    private RaycastHit hit;
    [SerializeField] private Rigidbody rb;
    
    [Header("Controller")]
    private CharacterController controller;
    [SerializeField] private int ClickValue = 0; //Checks if the Click action is being held or not 

    [Header("Mess Around Variables")] [SerializeField]
    private Transform TestSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ClickValue == 1)
        {
            TheClickMethod();
        }
        
      //  Vector2 mouseDeta = Mouse.current.position.ReadValue();
    }

    public void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit);
            if (hit.collider.tag == "Puck")
            {
                StartPos = Mouse.current.position.ReadValue();
                rb = hit.collider.gameObject.GetComponent<Rigidbody>();
            }
            
        }

        else
        {
            if (hit.collider.tag == "Puck")
            {
                Vector3 direction = new Vector3();
                EndPos = Mouse.current.position.ReadValue();
                direction = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).normalized;
                float mag = new Vector3(StartPos.x - EndPos.x, 0f, StartPos.y - EndPos.y).magnitude;
                float clampedMag = Mathf.Clamp(mag, 0, MaxLength);
                rb.AddForce(clampedMag * direction * Force);
                // Debug.Log("Released");
                hit = new RaycastHit();
            }
            
        }
    }

    private void TheClickMethod()
    {
       // Debug.Log("Hi daar");
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
