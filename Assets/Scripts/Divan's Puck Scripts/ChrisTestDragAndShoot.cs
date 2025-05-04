using Mirror;
using Mirror.Examples.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChrisTestDragAndShoot : NetworkBehaviour
{
    //Reference mirror billiards example
    private bool Mouse(out Vector3 mousePos)
    {
        Ray ray = GameObject.Find("PlayerCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            mousePos = ray.GetPoint(distance);
            return true;
        }
        mousePos = default;
        return false;
    }

    [ClientCallback]
    private void OnMouseDrag()
    {
        if (!Mouse(out Vector3 currentPos))
        {
            return;
        }
    }

    [Command(requiresAuthority =false)]
    private void CmdForce(Vector3 frc) 
    {
        transform.GetComponent<Rigidbody>().AddForce(frc, ForceMode.Impulse);
    }

    [ClientCallback]
    private void OnMouseUp()
    {
        if (!Mouse(out Vector3 currentPos))
        {
            return;
        }

        Vector3 startPos = transform.position;
        Vector3 d = startPos - currentPos;
        Vector3 force = d * 10;
        force = Vector3.ClampMagnitude(force, 50);

        CmdForce(force);
    }
}
