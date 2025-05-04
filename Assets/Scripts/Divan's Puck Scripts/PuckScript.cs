using UnityEngine;
using Mirror;

public class PuckScript : NetworkBehaviour
{
    [Header("Drag and Store variables")]
    [SyncVar]public bool canDrag = false;
    [SyncVar]public bool isStore;
    [SerializeField] private float minX, maxX, minZ, maxZ;
    
    [Header("Puck variables")]
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Collider coll;

    [SerializeField] private GameObject onHitTower;
    [SerializeField] private GameObject onHitPuck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void Drag(float clampedMag, Vector3 direction ,float mouseForce)
    {
        rb.AddForce(clampedMag * direction * mouseForce);
        Debug.Log("did it");
        canDrag = false;
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

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Tower")
        {
            GameObject system = Instantiate(onHitTower, collision.contacts[0].point, onHitTower.transform.rotation);
            NetworkServer.Spawn(system);
        }
        
        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Puck")
        {
            GameObject system = Instantiate(onHitPuck, collision.contacts[0].point, onHitTower.transform.rotation);
            NetworkServer.Spawn(system);
        }
    }
}
