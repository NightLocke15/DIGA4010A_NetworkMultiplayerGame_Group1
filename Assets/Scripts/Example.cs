using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
public class Example : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private float moveDirection = 0f;
    public Transform playerCamera;
    public UIManager uiManager;
    [ClientCallback]
    void Start()
    {
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    [ClientCallback] //Run this to run on the client only, not all at once. So things like Update(), Start(), OnTrigger and so on.
                     //So thing sonly revelant to the player not the server. Good optimization. Don't need a million updates
    private void Update()
    {
        if (!isLocalPlayer) return;
        transform.Translate(Vector3.right * moveDirection * moveSpeed *
        Time.deltaTime);
    }

    [TargetRpc] //Runs a specfic method for a specfic client. Things like secert code. 
    public void TargetShowWelcomeMessage(NetworkConnection target, string message)
    {
        if (uiManager != null)
        {
            uiManager.ShowMessage(message);
        }
    }
    [Command] //used to ask the server something 
    void CmdChangeMyColour()
    {
        Color newColor = Random.ColorHSV();
        RpcChangeColor(newColor);
    }

    [ClientRpc] //after [Command] this syncs this to all the other servers
    void RpcChangeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
    public void OnColorChange(InputValue value)
    {
        if (value.isPressed)
        {
            CmdChangeMyColour();
        }
    }
    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>().x;
    }
}

