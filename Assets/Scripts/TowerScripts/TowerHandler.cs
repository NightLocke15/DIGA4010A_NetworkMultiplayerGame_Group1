using UnityEngine;

public class TowerHandler : MonoBehaviour
{
    private EnemySpawning spawningScript;
    [SerializeField] private int towerHealth = 100;

    private void Start()
    {
        spawningScript = GameObject.Find("SceneManager").GetComponent<EnemySpawning>();
    }

    private void Update()
    {
        if (towerHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            towerHealth -= 10;
            Destroy(collision.gameObject); //Destroy the enemy when it hit's the tower
        }

        if (collision.collider.tag == "Ally")
        {
            towerHealth -= 10;
        }
    }
}
