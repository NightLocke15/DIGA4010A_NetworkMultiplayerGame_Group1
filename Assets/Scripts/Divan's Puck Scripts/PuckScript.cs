using UnityEngine;

public class PuckScript : MonoBehaviour
{
    [Header("Drag and Store variables")]
    public bool canDrag = false;
    public bool isStore;
    [SerializeField] private float minX, maxX, minZ, maxZ;
    
    [Header("Puck variables")]
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CannotBeDrag()
    {
        canDrag = false;
        isStore = true;
    }

    public void CanBeDrag()
    {
        canDrag = true;
        isStore = false;
    }

    public void ChangePosToStorage(Transform newPos)
    {
        transform.parent = null;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        float adjustX = Random.Range(minX, maxX);
        float adjustZ = Random.Range(minZ, maxZ);
        Vector3 adjustPos = new Vector3(newPos.position.x + adjustX, newPos.position.y, newPos.position.z + adjustZ );
        transform.position = adjustPos;
        
        //rb.MovePosition(adjustPos);
        transform.parent = newPos;
     //   transform.localPosition = new Vector3(0, 0, 0);
        CannotBeDrag();
      
    }

    public void ChangePosToBoard(Transform newPos)
    {
        
        CanBeDrag();
        
        transform.parent = newPos;
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.localPosition= new Vector3(0, 0, 0);
    }
}
