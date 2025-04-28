using UnityEngine;

public class TowerHandler : MonoBehaviour
{
    private EnemySpawning spawningScript;

    private void Start()
    {
        spawningScript = GameObject.Find("SceneManager").GetComponent<EnemySpawning>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Destroy(collision.gameObject); //Destroy the enemy when it hit's the tower
        }
    }
}
