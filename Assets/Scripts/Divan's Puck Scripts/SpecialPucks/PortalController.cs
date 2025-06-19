using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class PortalController : NetworkBehaviour
{
    [SerializeField] private Transform storeLoc;
    public List<GameObject> portalPucks = new List<GameObject>();

    [SerializeField] private GameObject goblinPuck, orcPuck, ogrePuck;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void SetStoreLoc(Transform storeLoc)
    {
        this.storeLoc = storeLoc;
    }

    [Server]
    public void AddPuckToList(GameObject puck)
    {
        portalPucks.Add(puck);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Puck")
        {
            StoreThePuck(collision.gameObject.GetComponent<PuckScript>());
            RemovePuckFromList();
        }
        
        else if (collision.gameObject.tag == "Enemy")
        {
            ECscript eCscript = collision.gameObject.GetComponent<ECscript>();
            StoreTheEnemyPuck(eCscript);
            eCscript.DeleteStuff();
            RemovePuckFromList();
        }
    }

    [Server]
    private void RemovePuckFromList()
    {
        NetworkServer.Destroy(portalPucks[0]);

        if (portalPucks.Count < 1)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
    
    [Server]
    public void StoreThePuck(PuckScript script)
    {
        script.ChangePosToStorage(storeLoc);
    }

    [Server]
    public void StoreTheEnemyPuck(ECscript ecscript)
    {
        ECscript.EnemyTypes type = ecscript.enemyType;
        GameObject puck = new GameObject();
        switch (type)
        {
            case ECscript.EnemyTypes.Goblin:
                puck = goblinPuck;
                break;
            case ECscript.EnemyTypes.Orc:
                puck = orcPuck;
                break;
            case ECscript.EnemyTypes.Ogre:
                puck = ogrePuck;
                break;
            default:
                break;
        }
        
        GameObject instantiatedPuck = Instantiate(puck);
        if (ecscript.isLeader == true)
        {
            switch (type)
            {
                case ECscript.EnemyTypes.Goblin:
                    instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Magnet;
                    break;
                case ECscript.EnemyTypes.Orc:
                    instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Healer;
                    break;
                case ECscript.EnemyTypes.Ogre:
                    instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Portal;
                    break;
                default:
                    break;
            }
        }

        else
        {
            instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Normal;
        }
        
        NetworkServer.Spawn(instantiatedPuck);
        instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storeLoc);;
            
        
    }
}
