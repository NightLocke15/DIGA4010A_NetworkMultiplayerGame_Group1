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
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private TextMeshProUGUI connectionStatus;
    public bool disconnected;
    private float time;
    [SerializeField] private GameObject playerOne;
    [SerializeField] private GameObject playerTwo;

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
            playerOne = GameObject.Find("PlayerOne(Clone)");
            playerTwo = GameObject.Find("PlayerTwo(Clone)");
        }
        else
        {
            waitingP1.SetActive(true);
            ready.SetActive(false);
        }

    }

    private void Update()
    {
        if (disconnected)
        {
            time += Time.deltaTime;
        }

        if (time > 1f)
        {
            disconnected = false;
            connectionPanel.SetActive(false);
            time = 0;            
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
        connectionPanel.SetActive(true);
        connectionStatus.text = "Player Disconnected";
        disconnected = true;
        SceneManager.LoadScene(0);
        base.OnClientDisconnect();

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        connectionPanel.SetActive(true);
        connectionStatus.text = "Player Disconnected";
        disconnected = true;
        SceneManager.LoadScene(0);
        base.OnServerDisconnect(conn);
    }

    public void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer"+ name);
    }
}
