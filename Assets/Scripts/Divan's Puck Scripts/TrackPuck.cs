using Mirror;
using UnityEngine;

public class TrackPuck : MonoBehaviour
{
    private Vector3 position;

    //[ClientRpc]
    void Update()
    {
        position = transform.position;
    }
}
