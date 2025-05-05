using Mirror;
using UnityEngine;

public class TowerHandler : NetworkBehaviour
{
    [SerializeField] private EnemySpawning spawningScript;
    [SyncVar]
    public int towerHealth = 100;
    [SerializeField] private ParticleSystem onHitTower;

    private void Start()
    {
     
    }

    private void Update()
    {
        
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            towerHealth -= 10;
            DestroyEnemyCmd(collision.gameObject);
            if (towerHealth <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        if (collision.collider.tag == "Puck")
        {
            towerHealth -= 10;
            if (towerHealth <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void DestroyEnemyCmd(GameObject gameObject)
    {
        DestroyEnemyRpc(gameObject);
    }

    [Server]
    public void DestroyEnemyRpc(GameObject gameObject)
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject); //Destroy the enemy when it hit's the tower
        }
        
    }
}
