using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Telepathy;

public class CustomNetworkManager : NetworkManager
{
    public List<GameObject> playerPrefabs = new List<GameObject>();
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject Manager;
    [SerializeField] private GameObject waitingP1;
    [SerializeField] private GameObject waitingP2;  
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject menuPanel;
    public bool disconnected;
    private float time;
    [SerializeField] private GameObject disconnectedPanel;

    public override void OnServerAddPlayer(NetworkConnectionToClient connection)
    {
        if (playerPrefabs.Count == 0)
        {
            return;
        }

        int index = numPlayers % playerPrefabs.Count;
        GameObject selectedPlayer = playerPrefabs[index];
        GameObject player = Instantiate(selectedPlayer);
        NetworkServer.AddPlayerForConnection(connection, player);

        if (numPlayers == 2) //Checks if the second player has joined
        {
            menuCanvas.GetComponent<MenuUI>().startButton.gameObject.SetActive(true);
            ready.SetActive(true);
            waitingP1.SetActive(false);
            //Manager.GetComponent<TurnOrderManager>().FirstTurn();
            Manager.GetComponent<TurnOrderManager>().AssignParents(numPlayers);
        }
        else
        {
            waitingP1.SetActive(true);
            ready.SetActive(false);
        }

    }

    //public override void OnStopHost()
    //{
    //    StopHost();
    //    connectionPanel.SetActive(true);
    //    connectionStatus.text = "Player Disconnected";
    //    disconnected = true;
    //    SceneManager.LoadScene(0);
    //    base.OnStopHost();
    //}

    //public override void OnStopServer()
    //{
    //    StopServer();
    //    connectionPanel.SetActive(true);
    //    connectionStatus.text = "Player Disconnected";
    //    disconnected = true;
    //    SceneManager.LoadScene(0);
    //    base.OnStopServer();
    //}

    //public override void OnStopClient()
    //{
    //    StopClient();
    //    connectionPanel.SetActive(true);
    //    connectionStatus.text = "Player Disconnected";
    //    disconnected = true;
    //    SceneManager.LoadScene(0);
    //    base.OnStopClient();
    //}

    public override void OnClientDisconnect()
    {
        if (NetworkClient.active)
        {
            StopClient();
        }
        
        disconnectedPanel.SetActive(true);
        Debug.Log("Client Gone");
        base.OnClientDisconnect();

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (NetworkServer.active)
        {
            StopServer();
            StopClient();
        }
        disconnectedPanel.SetActive(true);
        Debug.Log("Server Gone");
        base.OnServerDisconnect(conn);
    }

    public void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer"+ name);
    }

    public void MenuDisconnect()
    {
        menuPanel.SetActive(true);
        disconnectedPanel.SetActive(false);
    }
}
