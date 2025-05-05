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
        if (towerHealth <= 0)
        {
           NetworkServer.Destroy(gameObject);
        }
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            towerHealth -= 10;
            DestroyEnemyCmd(collision.gameObject);
        }

        if (collision.collider.tag == "Puck")
        {
            towerHealth -= 10;
        }
    }

    [Command(requiresAuthority = false)]
    public void DestroyEnemyCmd(GameObject gameObject)
    {
        DestroyEnemyRpc(gameObject);
    }

    [ClientRpc]
    public void DestroyEnemyRpc(GameObject gameObject)
    {
        
        NetworkServer.Destroy(gameObject); //Destroy the enemy when it hit's the tower
    }
}
