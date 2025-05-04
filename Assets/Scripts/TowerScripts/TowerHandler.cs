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
            Destroy(gameObject);
        }
    }

    //[ClientCallback]
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.tag == "Enemy")
    //    {
    //        towerHealth -= 10;
    //        ParticleSystem system = Instantiate(onHitTower, collision.contacts[0].point, onHitTower.transform.rotation);
    //        Destroy(collision.gameObject); //Destroy the enemy when it hit's the tower
    //    }

    //    if (collision.collider.tag == "Puck")
    //    {
    //        ParticleSystem system = Instantiate(onHitTower, collision.contacts[0].point, onHitTower.transform.rotation);
    //        towerHealth -= 10;
    //    }
    //}    
}
