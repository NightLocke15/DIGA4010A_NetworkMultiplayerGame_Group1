using Mirror;
using UnityEngine;

public class DestroyParticle : NetworkBehaviour
{
    private float timer;

    [ServerCallback]
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 3)
        {
            if (NetworkServer.active)
            {
                NetworkServer.Destroy(gameObject);
            }
            
        }
    }


}
