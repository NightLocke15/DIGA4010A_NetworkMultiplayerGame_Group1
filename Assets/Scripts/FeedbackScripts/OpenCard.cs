using Mirror;
using UnityEngine;

public class OpenCard : NetworkBehaviour
{
    private Vector3 target;
    private bool open = false;
    private AudioSource audioSource;

    private void Start()
    {
        target = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5);
    }

    public void OnMouseDown()
    {
        CmdOpenCard();
    }

    [Command (requiresAuthority = false)]
    public void CmdOpenCard()
    {
        RpcOpenCard();
    }

    [ClientRpc]
    public void RpcOpenCard()
    {
        if (open)
        {
            target = new Vector3(transform.position.x, -3.9f, transform.position.z);
            open = false;
            audioSource.Play();
        }
        else
        {
            target = new Vector3(transform.position.x, 3f, transform.position.z);
            open = true;
            audioSource.Play();
        }
    }
}
