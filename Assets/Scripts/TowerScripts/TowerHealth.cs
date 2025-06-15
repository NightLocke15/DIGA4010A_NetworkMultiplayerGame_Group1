using Mirror;
using UnityEngine;

public class TowerHealth : NetworkBehaviour
{
    private TowerHandler towerHandler;
    [SyncVar]
    public bool floored;

    private void Start()
    {
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            floored = true;
        }
        
        // if (floored)
        // {
        //     if (collision.collider.tag == "Enemy")
        //     {
        //         towerHandler.LoseHealth();
        //         CmdKillEnemy(collision.collider.gameObject);
        //         CmdLoseHealth();
        //     }
        //     
        //     if (collision.collider.tag == "Puck")
        //     {
        //         towerHandler.LoseHealth();
        //         CmdKillEnemy(collision.collider.gameObject);
        //         CmdLoseHealth();
        //     }
        // }        
    }

    public void TheTowerWasHit(GameObject deletePuck)
    {
        towerHandler.LoseHealth();
       // CmdKillEnemy(deletePuck);
       DestroyCollPuck(deletePuck);
        //CmdLoseHealth();
        DestroyYourself();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Floor")
        {
            floored = true;
        }
    }

    [Command(requiresAuthority = false)]
    private void DestroyYourself()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    private void DestroyCollPuck(GameObject deletePuck)
    {
        NetworkServer.Destroy(deletePuck);
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
