using UnityEngine;
using Mirror;
using Unity.Cinemachine;
using Mirror.BouncyCastle.Crypto.Digests;

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
    private bool shake;
    private float shakeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shake == true)
        {
            shakeTime += Time.deltaTime;

            if (isServer)
            {
                GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
                GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 1;
            }
            else
            {
                GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 1;
                GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 1;
            }
            
        }

        if (shakeTime > 0.2f)
        {
            shake = false;
            shakeTime = 0;

            if (isServer)
            {
                GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
                GameObject.Find("CinemachineCameraOne").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0;
            }
            else
            {
                GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = 0;
                GameObject.Find("CinemachineCameraTwo").GetComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = 0;
            }
        }
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
            TowerHit(collision.contacts[0].point);
        }
        
        if (collision.collider.tag == "Enemy" || collision.collider.tag == "Puck")
        {
            PuckHit(collision.contacts[0].point);
        }

        if (collision.collider.tag == "Floor")
        {
            shake = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void TowerHit(Vector3 pos)
    {
        TowerHitRpc(pos);
    }

    [ClientRpc]
    public void TowerHitRpc(Vector3 pos)
    {
        GameObject system = Instantiate(onHitTower, pos, onHitTower.transform.rotation);
        NetworkServer.Spawn(system);
    }

    [Command(requiresAuthority = false)]
    public void PuckHit(Vector3 pos)
    {
        PuckHitRpc(pos);
    }

    [ClientRpc]
    public void PuckHitRpc(Vector3 pos)
    {
        GameObject system = Instantiate(onHitPuck, pos, onHitTower.transform.rotation);
        NetworkServer.Spawn(system);
    }

    
}
