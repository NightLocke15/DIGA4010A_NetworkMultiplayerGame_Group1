using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class CustomNetworkManager : NetworkManager
{
    public List<GameObject> playerPrefabs = new List<GameObject>();
    [SerializeField] private GameObject menuCanvas;

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
            Manager.GetComponent<TurnOrderManager>().FirstTurn();
            Manager.GetComponent<TurnOrderManager>().AssignParents(numPlayers);
        }
    }
    
    public void OnConnectedToServer()
    {
        Debug.Log("OnConnectedToServer"+ name);
    }
}
