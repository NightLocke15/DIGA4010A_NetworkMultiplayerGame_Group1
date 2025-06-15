using System.Collections;
using Mirror;
using UnityEngine;

public class TowerHandler : NetworkBehaviour
{
    [SyncVar]
    public int towerHealth = 10;
    [SerializeField] private ParticleSystem onHitTower;
    [SerializeField] private GameObject towerHealthDisc;
    private float height = 1;

    public void Start()
    {
       
    }

    private void Update()
    {
        
    }

    public void CallStart()
    {
        for (int i = 0; i < towerHealth; i++)
        {
            CmdSpawnTower();
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdSpawnTower()
    {
        RpcSpawnTower();
    }

    [ClientRpc]
    public void RpcSpawnTower()
    {
        if (isServer)
        {
            GameObject health = Instantiate(towerHealthDisc, new Vector3(4.26f, 10f + height, -61.93f), Quaternion.identity);
            NetworkServer.Spawn(health);
            height += 1;
        }
            
    }

    [Server]
    public void LoseHealth()
    {
        towerHealth -= 1;

        if (towerHealth <= 0)
        {
            TurnOrderManager turnOrderManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TurnOrderManager>();
            
            turnOrderManager.CmdEndScreen();
        }
    }
    //[ClientCallback]
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.tag == "Enemy")
    //    {
    //        towerHealth -= 10;
    //        DestroyEnemyCmd(collision.gameObject);
    //        if (towerHealth <= 0)
    //        {
    //            NetworkServer.Destroy(gameObject);
    //        }
    //    }

    //    if (collision.collider.tag == "Puck")
    //    {
    //        towerHealth -= 10;
    //        if (towerHealth <= 0)
    //        {
    //            NetworkServer.Destroy(gameObject);
    //        }
    //    }
    //}

    //[Command(requiresAuthority = false)]
    //public void DestroyEnemyCmd(GameObject gameObject)
    //{
    //    DestroyEnemyRpc(gameObject);
    //}

    //[Server]
    //public void DestroyEnemyRpc(GameObject gameObject)
    //{
    //    if (isServer)
    //    {
    //        NetworkServer.Destroy(gameObject); //Destroy the enemy when it hit's the tower
    //    }
        
    //}
}
