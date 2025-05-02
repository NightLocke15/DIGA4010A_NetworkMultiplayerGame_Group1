using UnityEngine;

public class Goals : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ally" || other.tag == "Puck" || other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}
