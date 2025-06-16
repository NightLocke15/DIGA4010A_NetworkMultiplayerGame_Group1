using System.Collections;
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
    private AudioSource audioSource;
    private GameObject healthItem;

    public void Start()
    {
       audioSource = GetComponent<AudioSource>();
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
        towerHealth -= 1;

        if (towerHealth <= 0)
        {
            TurnOrderManager turnOrderManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TurnOrderManager>();
            
            turnOrderManager.CmdEndScreen();
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
        audioSource.Play();

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
