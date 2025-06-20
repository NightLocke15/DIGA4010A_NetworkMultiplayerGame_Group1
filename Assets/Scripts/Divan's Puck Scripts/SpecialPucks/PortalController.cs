using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public class PortalController : NetworkBehaviour
{
    public Transform storeLocation;
    public List<GameObject> portalPucks = new List<GameObject>();

    [SerializeField] private GameObject goblinPuck, orcPuck, ogrePuck;
    
    [SerializeField]private Material player1Material, player2Material;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (storeLocation == null)
        {
            
        }
        else
        {
                for (int i = 0; i < portalPucks.Count; i++)
                {
                    if (!isServer)
                    {
                        AddPuckAsChild(i);
                    }
                    SetMaterialOnPP(i);
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
//        Debug.Log("Added to list");
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
        // Debug.Log(collision.gameObject.name);
        // if (collision.gameObject.tag == "Puck")
        // {
        //     Debug.Log(collision.gameObject.name);
        //     //StoreThePuck(collision.gameObject.GetComponent<PuckScript>());
        //     collision.gameObject.GetComponentInChildren<PuckScript>().ChangePosToStorage(storeLocation);
        //     RemovePuckFromList();
        // }
        //
        // else if (collision.gameObject.tag == "Enemy")
        // {
        //     ECscript eCscript = collision.gameObject.GetComponent<ECscript>();
        //     StoreTheEnemyPuck(eCscript);
        //     eCscript.DeleteStuff();
        //     RemovePuckFromList();
        // }
    }

    [Server]
    public void RemovePuckFromList()
    {
        NetworkServer.Destroy(portalPucks[0]);
        portalPucks.Remove(portalPucks[0]);

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

    [Command(requiresAuthority = false)]
    public void StoreTheEnemyPuck(ECscript ecscript)
    {
        if (ecscript != null)
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
            NetworkServer.Spawn(instantiatedPuck);
        
            if (ecscript.isLeader == true)
            {
                RpcPortalSetVariant(instantiatedPuck, ecscript);
            }

            else
            {
                SetNormalH(instantiatedPuck, ecscript);
            }
        
        
            instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storeLocation);
        
            ecscript.DeleteStuff();
        }
        
            
        
    }
    
    [ClientRpc]
    private void SetNormalH(GameObject instantiatedPuck, ECscript ecscript)
    {
        instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Normal;
        if (instantiatedPuck.GetComponent<PuckScript>().leaderCircle != null)
        {
            instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(false);
        }
        
        if (instantiatedPuck.GetComponent<PuckScript>().portalPuck != null)
        {
            instantiatedPuck.GetComponent<PuckScript>().portalPuck.canCreatePortal = false;
        }
        
        if (instantiatedPuck.GetComponent<PuckScript>().magnetPuck != null)
        {
            instantiatedPuck.GetComponent<PuckScript>().magnetPuck.canMagnet = false;
        }
        // instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(false);
    }

    [ClientRpc]
    private void RpcPortalSetVariant(GameObject instantiatedPuck, ECscript ecscript)
    {
        ECscript.EnemyTypes type = ecscript.enemyType;
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
        instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(true);
    }
}
