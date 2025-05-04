using UnityEngine;
using Mirror;

public class Goals : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ally" || other.tag == "Puck" || other.tag == "Enemy")
        {
            CmdDestroyPuck(other.gameObject);
        }
    }

    [Command]
    public void CmdDestroyPuck(GameObject puck)
    {
        Destroy(puck);
    }
}
