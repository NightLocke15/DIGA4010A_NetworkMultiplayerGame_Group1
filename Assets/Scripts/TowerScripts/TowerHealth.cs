using Mirror;
using UnityEngine;

public class TowerHealth : NetworkBehaviour
{
    private TowerHandler towerHandler;
    [SyncVar]
    private bool floored;

    private void Start()
    {
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (floored)
        {
            if (collision.collider.tag == "Enemy")
            {
                towerHandler.towerHealth -= 1;
                CmdKillEnemy(collision.collider.gameObject);
                CmdLoseHealth();
            }

            if (collision.collider.tag == "Puck")
            {
                towerHandler.towerHealth -= 1;
                CmdLoseHealth();
            }
        }        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Floor")
        {
            floored = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdLoseHealth()
    {
        if (isServer)
        {
            RpcLoseHealth();
        }        
    }

    [ClientRpc]
    public void RpcLoseHealth()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    public void CmdKillEnemy(GameObject enemy)
    {
        if (isServer)
        {
            RpcKillEnemy(enemy);
        }
    }

    [ClientRpc]
    public void RpcKillEnemy(GameObject enemy)
    {
        NetworkServer.Destroy(enemy);
    }
}
