using UnityEngine;
using Mirror;

public class ECscript : NetworkBehaviour
{
    [SerializeField] private EnemyTypes enemyType;
    [SerializeField] private Transform followAgent;
    [SerializeField] private TurnOrderManager turnOrderManager;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private int TurnOrder = 0;
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

            else
            {
                
            }
        }

        else
        {
            turnOrderManager = GameObject.FindWithTag("Manager").GetComponent<TurnOrderManager>();
        }
    }
}
