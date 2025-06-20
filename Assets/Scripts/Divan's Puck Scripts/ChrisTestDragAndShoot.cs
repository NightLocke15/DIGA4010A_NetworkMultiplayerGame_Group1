using Mirror;
using Mirror.Examples.Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChrisTestDragAndShoot : NetworkBehaviour
{
    private LineRenderer aimLine;

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
    private void OnMouseDown()
    {
        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, transform.position);
        aimLine.gameObject.SetActive(true);
    }

    [ClientCallback]
    private void OnMouseDrag()
    {
        if (!Mouse(out Vector3 currentPos))
        {
            return;
        }

        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, new Vector3((transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).x,
                            transform.position.y,
                            (transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).z));

    }

    [ClientCallback]
    private void OnMouseUp()
    {
        if (!Mouse(out Vector3 currentPos))
        {
            return;
        }

        aimLine.gameObject.SetActive(false);
    }
}
