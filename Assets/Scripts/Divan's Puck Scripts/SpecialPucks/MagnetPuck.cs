using Mirror;
using UnityEditor;
using UnityEngine;

public class MagnetPuck : NetworkBehaviour
{
    [Header("Magnet Puck")]
    public bool canMagnet = false;
    [SerializeField] private float magForce = 100f;
    [SerializeField] private Rigidbody ownRb;

    [Header("Spere variables")] [SerializeField]
    private float radius = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canMagnet = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [Command(requiresAuthority = false)]
    public void Cmd_deActivateMag()
    {
        RPC_DeActivateMag();
    }

    [ClientRpc]
    private void RPC_DeActivateMag()
    {
            canMagnet = false;
    }
    
    [Command(requiresAuthority = false)]
    public void Cmd_ActivateMag()
    {
        RPC_ActivateMag();
    }

    [ClientRpc]
    private void RPC_ActivateMag()
    {
        canMagnet = true;
    }

    void FixedUpdate()
    {
        
        if (canMagnet)
        {
            ApplyMagnet();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    [Server]
    private void ApplyMagnet()
    {
        Collider[] hitcolliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < hitcolliders.Length; i++)
        {
            if (hitcolliders[i].CompareTag("Puck") || hitcolliders[i].CompareTag("Enemy"))
            {
                Rigidbody rb = hitcolliders[i].GetComponent<Rigidbody>();
                Vector3 direction = (transform.position - rb.position).normalized;
                float distance = Vector3.Distance(hitcolliders[i].transform.position, transform.position);
                if (distance > 0)
                {
                    Debug.Log(rb.name);
                    Vector3 pullForce = direction/distance * magForce;
                    rb.AddForce(pullForce, ForceMode.Force);
                }
                
                
            }
        }
    }
}
