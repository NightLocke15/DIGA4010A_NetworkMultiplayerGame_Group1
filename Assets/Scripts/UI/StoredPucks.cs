using Mirror;
using TMPro;
using UnityEngine;

public class StoredPucks : NetworkBehaviour
{
    [SyncVar]
    public string storedPucksP1;
    [SyncVar]
    public string storedPucksP2;

    [SerializeField] private TextMeshPro playerOnePucks;
    [SerializeField] private TextMeshPro playerTwoPucks;

    [SerializeField] private GameObject playerOneStorage;
    [SerializeField] private GameObject playerTwoStorage;

    private void Update()
    {
        CmdStorePucks();
    }

    [Command (requiresAuthority = false)]
    public void CmdStorePucks()
    {
        RpcStoredPucks();
    }

    [ClientRpc]
    public void RpcStoredPucks()
    {
        storedPucksP1 = playerOneStorage.transform.childCount + " Pucks Stored";
        storedPucksP2 = playerTwoStorage.transform.childCount + " Pucks Stored";

        playerOnePucks.text = storedPucksP1;
        playerTwoPucks.text = storedPucksP2;
    }
}
