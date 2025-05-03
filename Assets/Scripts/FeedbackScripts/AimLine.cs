using UnityEngine;
using Mirror;

public class AimLine : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    private Vector3 mousePos;
    private Vector3 worldPos;
    private GameObject chosenPuck;
    private LineRenderer aimLine;

    private void Start()
    {
        aimLine.enabled = false;
    }

    private void Update()
    {
        //https://stackoverflow.com/questions/75603761/unity-screentoworldpoint-function-always-returns-the-camera-position-even-with-a
        if (isLocalPlayer)
        {
            mousePos = this.gameObject.GetComponent<DragAndShoot>().currentMouse.position.ReadValue();
        }
        
        Ray ray = playerCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            worldPos = hit.point;

            if (hit.collider.tag == "Puck")
            {
                chosenPuck = hit.collider.gameObject;
                Debug.Log(chosenPuck.name);
                aimLine = chosenPuck.transform.GetChild(0).GetComponent<LineRenderer>();               
            }

            if (chosenPuck != null)
            {
                if (Input.GetMouseButton(0)) 
                {
                    if (aimLine != null)
                    {
                        aimLine.enabled = true;
                        aimLine.SetPosition(0, chosenPuck.transform.position);
                        aimLine.SetPosition(1, new Vector3((chosenPuck.transform.position - (worldPos - chosenPuck.transform.position).normalized * (worldPos - chosenPuck.transform.position).magnitude * 2).x,
                            chosenPuck.transform.position.y,
                            (chosenPuck.transform.position - (worldPos - chosenPuck.transform.position).normalized * (worldPos - chosenPuck.transform.position).magnitude * 2).z));
                    }
                }
                else if (Input.GetMouseButtonUp(0)) 
                {
                    if (aimLine != null)
                    {
                        aimLine.enabled = false;
                    }                        
                }
                
            }
            
        }

        
    }
}
