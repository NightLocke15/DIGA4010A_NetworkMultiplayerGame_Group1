using System;
using UnityEngine;
using Mirror;
using Unity.Cinemachine;
using Mirror.BouncyCastle.Crypto.Digests;
using Telepathy;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class PuckScript : NetworkBehaviour
{
    [Header("Drag and Store variables")]
    [SyncVar]public bool canDrag = false;
    [SyncVar]public bool isStore;
    [SerializeField] private float minX, maxX, minZ, maxZ;
    
    [Header("Puck variables")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;

    [SerializeField] private GameObject onHitWall;
    [SerializeField] private GameObject onHitPuck;

    [Header("Move Puck variables")]
    [SerializeField] public float clampX = 5f, clampZ = 2.5f;
    //[SerializeField] private Outline outline;
    private CustomNetworkManager networkManager;
    private TowerHandler towerHandler;


    [Header("Special Pucks")] 
    public puckVariants variant;

    public PortalPuck portalPuck;
    public enum puckVariants
    {
        Normal,
        Magnet,
        Portal,
        Healer
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
        if (gameObject.GetComponent<PortalPuck>() != null)
        {
            portalPuck = gameObject.GetComponent<PortalPuck>();
        }
        //outline.enabled = false;

        //  ChangePosToStorage(transform.parent);
    }

    // Update is called once per frame
    [ClientCallback]
    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void CmdMoveThePuck(Vector3 PlacePos, float Inverse, float moveSpeed, Vector3 inputDirection, float radius) //We move the puck on the server
    {
        Vector3 anchorPos = Vector3.zero; //This pos is used to calculate the magnitude
        Vector3 anchorPoint = PlacePos; //This is the point we clamp the puck to
        Vector3 AdjustIP = new Vector3(rb.position.x - anchorPoint.x, rb.position.y - anchorPoint.y, rb.position.z - anchorPoint.z); //We create a new vector that checks were the puck is in relation to the anchorpoint
        Vector3 Initial = new Vector3(anchorPos.x + AdjustIP.x, anchorPos.y + AdjustIP.y, anchorPos.z + AdjustIP.z); //We adjust our initial puck position around the anchorpos in the same relation the puck is to the anchorpoint.
        Vector3 Movement = new Vector3(inputDirection.x * moveSpeed * Time.deltaTime * Inverse, 0f,
            inputDirection.z * moveSpeed * Time.deltaTime * Inverse); //We get the inputDirection with the speed
        
        Vector3 allowedPos = new Vector3(Initial.x + Movement.x, Initial.y + Movement.y, Initial.z + Movement.z); //We add movement to the initial pos
         Vector3 difference = new Vector3(allowedPos.x - anchorPos.x, allowedPos.y - anchorPos.y, allowedPos.z - anchorPos.z); //Get the difference between where we want to move and anchorPos
        // float mag = differnce.magnitude; //grab the magnitude to get the distance.
        // mag = Mathf.Clamp(mag, 0f, radius); //Clamp the magnitude, if restricting to a circle
        difference.x = Mathf.Clamp(difference.x, -clampX, clampX); //Clamps to a rectangle
        difference.z = Mathf.Clamp(difference.z, -clampZ, clampZ); //clamps to a rectangle
        
        Vector3 restrictPos = new Vector3();
        restrictPos = difference.normalized * difference.magnitude; //We used the clamped magnitude and it to the difference direction.

        Vector3 finalPos = new Vector3(anchorPoint.x + restrictPos.x, rb.position.y, anchorPoint.z + restrictPos.z); //We translate this back to the puck and anchorpoint's relation
        
        rb.MovePosition(finalPos); //Move the puck to that final Vector3
    }

    [Command(requiresAuthority = false)]
    public void Drag(float clampedMag, Vector3 direction ,float mouseForce)
    {
        rb.AddForce(clampedMag * direction * mouseForce);
        canDrag = false;
    }

    [Command(requiresAuthority = false)]
    public void CannotBeDrag()
    {
        //outline.enabled = false;
        canDrag = false;
        isStore = true;
    }

    [Command(requiresAuthority = false)]
    public void CanBeDrag()
    {
        //outline.enabled = true;
        canDrag = true;
        isStore = false;
    }

    [Command(requiresAuthority = false)]
    public void ChangePosToStorage(Transform newPos)
    {
        
        transform.parent = null;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        float adjustX = Random.Range(minX, maxX);
        float adjustZ = Random.Range(minZ, maxZ);
        Vector3 adjustPos = new Vector3(newPos.position.x + adjustX, newPos.position.y+2f, newPos.position.z + adjustZ );
        transform.position = adjustPos;
        
        //rb.MovePosition(adjustPos);
        //transform.parent = newPos;
        RpcChangePosToStorage(newPos);
     //   transform.localPosition = new Vector3(0, 0, 0);
        CannotBeDrag();
    }

    [ClientRpc]
    public void RpcChangePosToStorage(Transform newParent)
    {
        transform.parent = newParent;
        transform.gameObject.tag = "StoredPuck";
        
        // if (gameObject.GetComponent<EnemyController>())
        // {
        //     Destroy(gameObject.GetComponent<EnemyController>().TheOrc);
        //     Destroy(gameObject.GetComponent<EnemyController>());
        //     gameObject.AddComponent<NetworkRigidbodyReliable>();
        //     gameObject.AddComponent<NavMeshObstacle>();
        //   // puck.tag = "Puck";
        // }
    }

    [Command(requiresAuthority = false)]
    public void ChangePosToBoard(Transform newPos)
    {
        CanBeDrag();
        transform.parent = newPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition= new Vector3(0, 0, 0);
        RpcChangePosToStorage(newPos);
    }

    [ClientCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Tower")
        {
            towerHandler.TowerHit(collision.contacts[0].point);

            if (collision.gameObject.GetComponent<TowerHealth>())
            {
                if (collision.gameObject.GetComponent<TowerHealth>().floored)
                {
                    collision.gameObject.GetComponent<TowerHealth>().TheTowerWasHit(gameObject);
                }
            }
        }

        if (collision.collider.tag == "Wall") //Checks if we hit a wall
        {
            WallHit(collision.contacts[0].point); //Spawns VFX

            WallColl(); //Checks if a special collision should happen
        }

        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Puck") //Checks if we hit a puck
        {
            PuckHit(collision.contacts[0].point); //Spawns VFX
            PuckColl(collision.gameObject); //Checks if a special collision should happen
        }

        
    }

    private void WallColl() //Calls special puck functions on wall collisions
    {
        switch (variant)
        {
            case puckVariants.Normal:
                break;
            case puckVariants.Magnet:
                break;
            case puckVariants.Portal:
                Debug.Log("Wall Coll");
                portalPuck.SpawnThePortalPucks(transform); //Creates the portal pucks
                break;
            case puckVariants.Healer:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PuckColl(GameObject other) //Calls special wall collisions on puck collisons
    {
        switch (variant)
        {
            case puckVariants.Normal:
                portalPuck.SpawnThePortalPucks(transform); //Creates the portal pucks
                break;
            case puckVariants.Magnet:
                portalPuck.SpawnThePortalPucks(transform); //Creates the portal pucks
                break;
            case puckVariants.Portal:
                Debug.Log("Puck Coll");
                portalPuck.SpawnThePortalPucks(transform); //Creates the portal pucks
                if (other.GetComponent<ECscript>() != null)
                {
                   portalPuck.StoreTheEnemyPuck(other.GetComponent<ECscript>()); //Stores enemy puck
                   other.GetComponent<ECscript>().DeleteStuff();
                }
                break;
            case puckVariants.Healer:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [Command(requiresAuthority = false)]
    public void PuckHit(Vector3 pos)
    {
        PuckHitRpc(pos);
    }

    [Command(requiresAuthority = false)]
    public void WallHit(Vector3 pos)
    {
        WallHitRpc(pos);
    }

    [ClientRpc]
    public void PuckHitRpc(Vector3 pos)
    {
        if (isServer)
        {
            GameObject system = Instantiate(onHitPuck, pos, onHitPuck.transform.rotation);
            NetworkServer.Spawn(system);
        }
    }    

    [ClientRpc]
    public void WallHitRpc(Vector3 pos)
    {
        GameObject system = Instantiate(onHitWall, pos, onHitWall.transform.rotation);
        NetworkServer.Spawn(system);
    }
}
