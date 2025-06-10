using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class CustomNetworkManager : NetworkManager
{
    public List<GameObject> playerPrefabs = new List<GameObject>();
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject Manager;
    [SerializeField] private GameObject waitingP1;
    [SerializeField] private GameObject waitingP2;  
    [SerializeField] private GameObject ready;

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
    
    public void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer"+ name);
    }
}
