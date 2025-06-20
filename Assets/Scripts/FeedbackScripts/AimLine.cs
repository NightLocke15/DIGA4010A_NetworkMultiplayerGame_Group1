using UnityEngine;
using Mirror;
using static Unity.VisualScripting.Member;

public class AimLine : NetworkBehaviour
{
    [SerializeField] private LineRenderer aimLine;

    private void Start()
    {
        aimLine.gameObject.SetActive(false);
    }

    //Title: White Ball (From Billiards Example in Mirror
    //Author: Mirror Package (Open Source)
    //Date: 2 May 2025
    //Availability: In Examples folder in Mirror Package / https://mirror-networking.gitbook.io/docs/manual/examples/billiards 
    //Usage: Used it as reference to make aim line with line renderer
    // private bool Mouse(out Vector3 mousePos)
    // {
    //     Ray ray = GameObject.Find("PlayerCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
    //     Plane plane = new Plane(Vector3.up, transform.position);
    //
    //     if (plane.Raycast(ray, out float distance))
    //     {
    //         mousePos = ray.GetPoint(distance);
    //         return true;
    //     }
    //     mousePos = default;
    //     return false;
    // }

    // [ClientCallback]
    // private void OnMouseDown()
    // {
    //     if (transform.GetComponent<PuckScript>().canDrag == true)
    //     {
    //         aimLine.SetPosition(0, transform.position);
    //         aimLine.SetPosition(1, transform.position);
    //         aimLine.gameObject.SetActive(true);
    //     }        
    // }

    [ClientCallback]
    public void StartAimLine()
    {
        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1, transform.position);
        aimLine.gameObject.SetActive(true);
    }

    // [ClientCallback]
    // private void OnMouseDrag()
    // {
    //     if (!Mouse(out Vector3 currentPos))
    //     {
    //         return;
    //     }
    //
    //     if (transform.GetComponent<PuckScript>().canDrag == true)
    //     {
    //         aimLine.SetPosition(0, transform.position);
    //         aimLine.SetPosition(1, new Vector3((transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).x,
    //                             transform.position.y,
    //                             (transform.position - (currentPos - transform.position).normalized * (currentPos - transform.position).magnitude * 2).z));
    //     }
    // }

    public void UpdateAimLine(float clampedMag, Vector3 direction, float force, Vector3 startPos)
    {
        aimLine.SetPosition(0, transform.position);
        aimLine.SetPosition(1,new Vector3((transform.position.x+ (direction.x * clampedMag)), transform.position.y, (transform.position.z + (direction.z * clampedMag))));
    }

    // [ClientCallback]
    // private void OnMouseUp()
    // {
    //     if (!Mouse(out Vector3 currentPos))
    //     {
    //         return;
    //     }
    //
    //     aimLine.gameObject.SetActive(false);
    // }

    [ClientCallback]
    public void StopAimLine()
    {
        aimLine.gameObject.SetActive(false);
    }
}
