using UnityEngine;

public class TowerHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }
}
