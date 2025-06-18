using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TowerHandler : NetworkBehaviour
{
    [SyncVar]
    public int towerHealth = 10;
    //[SerializeField] private ParticleSystem onHitTower;
    [SerializeField] private GameObject towerHealthDisc;
    private float height = 1;
    [SerializeField] private GameObject onHitTower;
    [SerializeField] private AudioSource audioSource;
    private GameObject healthItem;
    [SerializeField] private TurnOrderManager turnOrderManager;

    public List<GameObject> towerHealthList = new List<GameObject>();

    public void Start()
    {
       
    }

    private void Update()
    {
        
    }

    public void CallStart()
    {
        CmdSpawnTower();
        
    }

    [Command (requiresAuthority = false)]
    public void CmdSpawnTower()
    {
        RpcSpawnTower();
        
    }

    [ClientRpc]
    public void RpcSpawnTower()
    {
        StartCoroutine(Spawning());
    }

    [Server]
    public void LoseHealth()
    {
        //towerHealth -= 1;

        

        if (towerHealth <= 0)
        {
            turnOrderManager.CmdEndScreen();
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdDeleteHealth(string health)
    {
        RpcDeleteHealth(health);
    }

    [ClientRpc]
    public void RpcDeleteHealth(string name)
    {

        Debug.Log("hit");
        if (isServer)
        {
            
            if (name == "ChrisPuck(Clone)" || name == "OrcPuck(Clone)" || name == "MediumEnemy(Clone)")
            {
                if (towerHealth >= 2)
                {
                    
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    towerHealth -= 2;
                }
                else
                {
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    towerHealth -= 1;
                }
                
            }
            else if (name == "GoblinPuck(Clone)" || name == "SmallEnemy")
            {
                NetworkServer.Destroy(towerHealthList[0]);
                towerHealthList.RemoveAt(0);
                towerHealth -= 1;
            }
            else if (name == "OgrePuck(Clone)" || name == "BigEnemy")
            {
                if (towerHealth >= 3)
                {
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    towerHealth -= 3;
                }
                else if (towerHealth == 2)
                {
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    towerHealth -= 2;
                }
                else
                {
                    NetworkServer.Destroy(towerHealthList[0]);
                    towerHealthList.RemoveAt(0);
                    towerHealth -= 1;
                }
                
            }
        }
    }

    private IEnumerator Spawning()
    {
        for (int i = 0; i < towerHealth; i++)
        {            
            if (isServer)
            {
                healthItem = Instantiate(towerHealthDisc, new Vector3(4.26f, 10f + height, -61.93f), Quaternion.identity);
                NetworkServer.Spawn(healthItem);
                towerHealthList.Add(healthItem);
            }
            
            //healthItem.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.3f);
        }       
    }

    [Command(requiresAuthority = false)]
    public void TowerHit(Vector3 pos)
    {
        TowerHitRpc(pos);
    }

    [ClientRpc]
    public void TowerHitRpc(Vector3 pos)
    {
        if (isServer)
        {
            GameObject system = Instantiate(onHitTower, pos, onHitTower.transform.rotation);
            NetworkServer.Spawn(system);            
        }
       // audioSource.Play();

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
