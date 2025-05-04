using UnityEngine;
using Mirror;

public class AimLine : NetworkBehaviour
{
    [SerializeField] private LineRenderer aimLine;

    private void Start()
    {
        aimLine.gameObject.SetActive(false);
    }

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
        if (transform.GetComponent<PuckScript>().canDrag == true)
        {
            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, transform.position);
            aimLine.gameObject.SetActive(true);
        }        
    }

    [ClientCallback]
    private void OnMouseDrag()
    {
        if (!Mouse(out Vector3 currentPos))
        {
            return;
        }

        if (transform.GetComponent<PuckScript>().canDrag == true)
        {
            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, new Vector3((transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).x,
                                transform.position.y,
                                (transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).z));
        }
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
