using UnityEngine;
using Mirror;

public class Ping_Script : NetworkBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool startTimer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTimer = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            lifetime -= Time.deltaTime;

            if (lifetime < 0)
            {
                CmdKillThePing();
            }
        }
    }


    [Server]
    private void KillThePing()
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdKillThePing()
    {
       KillThePing();
    }
}