using Mirror;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 3)
        {
            NetworkServer.Destroy(gameObject);
        }
    }


}
