using UnityEngine;
using Mirror;

public class ECscript : NetworkBehaviour
{
   [Header("The Enemy Info")]
    [SerializeField] private EnemyTypes enemyType;
    public float moveDistance;
    
    [Header("Delete variables")]
    [SerializeField] private Transform deleteTransform;
    [SerializeField] private GameObject deleteModel;
    [SerializeField] private GameObject deleteAgent;
    
    [Header("Other Varaibles")]
    [SerializeField] private Transform followAgent;
    public TurnOrderManager turnOrderManager;
    public Rigidbody rb;
    public int TurnOrder = 0;
    public enum EnemyTypes
    {
        Goblin,
        Orc,
        Ogre
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (turnOrderManager != null)
        {
            if (TurnOrder == 0)
            {
                rb.MovePosition(followAgent.position);
            }
        }

        else
        {
            turnOrderManager = GameObject.FindWithTag("Manager").GetComponent<TurnOrderManager>();
        }
    }

    public void DeleteStuff()
    {
        transform.parent = null;
        Destroy(deleteModel);
        Destroy(deleteAgent);
        Destroy(deleteTransform);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (deleteAgent != null)
        {
            if (other.tag == "Floor")
            {
                deleteAgent.SetActive(true);
            }
        }
    }
}
