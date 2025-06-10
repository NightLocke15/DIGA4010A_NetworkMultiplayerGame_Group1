using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class ECscript : NetworkBehaviour
{
   [Header("The Enemy Info")]
    public EnemyTypes enemyType;
    public float moveDistance;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AScript agentScript;
    
    [Header("Delete variables")]
    [SerializeField] private GameObject deleteTransform;
    [SerializeField] private GameObject deleteModel;
    [SerializeField] private GameObject deleteAgent;
    
    [Header("Other Varaibles")]
    [SerializeField] private Transform followAgent;
    public TurnOrderManager turnOrderManager;
    public Rigidbody rb;
    public int TurnOrder = 0;
    public bool canMove = false;
    public ESscript es_Script;
    
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
            if (TurnOrder == 0 && canMove)
            {
                rb.MovePosition(followAgent.position);
            }
        }
        
        else
        {
            turnOrderManager = GameObject.FindWithTag("Manager").GetComponent<TurnOrderManager>();
        }
    }

    [Command(requiresAuthority = false)]
    public void DeleteStuff()
    {
        DestroyYourself();
       // gameObject.AddComponent<NetworkIdentity>();
        //NetworkServer.Spawn(gameObject);
    }

    [ClientRpc]
    private void DestroyYourself()
    {
        es_Script.agentScripts.Remove(agentScript);
        NetworkServer.Destroy(deleteTransform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (agent != null)
        {
            if (collision.gameObject.tag == "Floor")
            {
                //Debug.Log(deleteAgent.name + "  " + this.name);
                deleteAgent.transform.localPosition = transform.localPosition;
                EnableAgentCmd();
            }
        }
    }


    [Command(requiresAuthority = false)]
    public void EnableAgentCmd()
    {
        RpcEnableAgent();
        //Debug.Log("The agent is enabled");
    }

    [ClientRpc]
    private void RpcEnableAgent()
    {
        agent.enabled = true;
        //Debug.Log("End");
    }
}
