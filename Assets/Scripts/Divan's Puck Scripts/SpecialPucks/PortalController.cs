using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class PortalController : NetworkBehaviour
{
    [FormerlySerializedAs("storeLoc")] [SerializeField] private Transform storeLocation;
    public List<GameObject> portalPucks = new List<GameObject>();

    [SerializeField] private GameObject goblinPuck, orcPuck, ogrePuck;
    
    [SerializeField]private Material player1Material, player2Material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (storeLocation == null)
        {
            Debug.Log("nothing to store location");
        }
        else
        {
                Debug.Log(storeLocation.name);
                for (int i = 0; i < portalPucks.Count; i++)
                {
                    SetMaterialOnPP(i);
                    if (!isServer)
                    {
                        AddPuckAsChild(i);
                    }
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void SetStoreLoc(Transform storeLoc)
    {
        RpcSetStoreLoc(storeLoc);
        //
    }

    [ClientRpc]
    private void RpcSetStoreLoc(Transform storeLoc)
    {
       // Debug.Log(storeLoc);
        storeLocation = storeLoc;
    }

    [Server]
    public void AddPuckToList(GameObject puck, int index)
    {
        Debug.Log("Added to list");
        RPC_AddPuckToList(puck, index);
        portalPucks.Add(puck);
     //   AddPuckAsChild(index);
       // SetMaterialOnPP(index);
    }

    [ClientRpc]
    private void RPC_AddPuckToList(GameObject puck, int index)
    {
        if (!isServer)
        {
            portalPucks.Add(puck);
        }
        
    }

   
    private void AddPuckAsChild(int puckIndex)
    {
        Debug.Log("Made child");
        portalPucks[puckIndex].transform.parent = this.transform;
    }

  
    private void SetMaterialOnPP(int index)
    {
        //Debug.Log(storeLocation.name + " Set material");
        // Debug.Log("Set material");
        if (storeLocation.name == "PL1_storage")
        {
           portalPucks[index].GetComponent<MeshRenderer>().material = player1Material;
        }
        
        else if (storeLocation.name == "PL2_Storage")
        {
            portalPucks[index].GetComponent<MeshRenderer>().material = player2Material;
        }
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
        script.ChangePosToStorage(storeLocation);
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
        instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storeLocation);;
            
        
    }
}
