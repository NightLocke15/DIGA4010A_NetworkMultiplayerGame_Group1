using Mirror;
using UnityEngine;

public class OpenCard : NetworkBehaviour
{
    private Vector3 target;
    private bool open = false;

    private void Start()
    {
        target = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5);
    }

    [ClientCallback]
    public void OnMouseDown()
    {
        if (open)
        {
            target = new Vector3(transform.position.x, -3.9f, transform.position.z);
            open = false;
        }
        else
        {
            target = new Vector3(transform.position.x, 3f, transform.position.z);
            open = true;
        }
    }
}
