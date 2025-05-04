using Mirror;
using UnityEngine;

public class TowerHandler : NetworkBehaviour
{
    [SerializeField] private EnemySpawning spawningScript;
    [SerializeField] private int towerHealth = 100;
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
            NetworkServer.Destroy(collision.gameObject); //Destroy the enemy when it hit's the tower
        }

        if (collision.collider.tag == "Puck")
        {
            towerHealth -= 10;
        }
    }    
}
