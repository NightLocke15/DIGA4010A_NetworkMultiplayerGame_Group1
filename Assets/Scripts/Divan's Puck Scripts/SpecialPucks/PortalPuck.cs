using UnityEngine;
using Mirror;
using UnityEngine.Serialization;

public class PortalPuck : NetworkBehaviour
{
    [Header("Portal")] [SerializeField]
    private GameObject portalPrefab, portalParent;

    [SerializeField] private GameObject goblinPuck;
     [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject orcPuck;

    [SerializeField] private Transform storeLocation;

    [SerializeField] private bool usePuckVersion = true;
    public bool canCreatePortal = false;

    [SerializeField] private int portalCount = 3;

    [SerializeField] private float adjustHeight = 2f;
    
    [SerializeField]private Material player1Material, player2Material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cmd_PortalBoolFalse();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command(requiresAuthority = false)]
    public void ReleasedInWild(Transform plStorePos)
    {
        RPC_ReleasedInWild(plStorePos);
    }

    [ClientRpc]
    private void RPC_ReleasedInWild(Transform plStorePos)
    {
       // Debug.Log("we did this command");
        canCreatePortal = true;
        storeLocation = plStorePos;
    }

    [Command(requiresAuthority = false)]
    public void SpawnThePortalPucks(Transform spawnPoint)
    {
        RPC_SpawnPortalPuck(spawnPoint);
    }

    [Server]
    private void RPC_SpawnPortalPuck(Transform spawnPoint)
    {
        //Debug.Log("Spawn before bool check");
        if (canCreatePortal)
        {
            GameObject porContainer = new GameObject();
            PortalController portalController = new PortalController();
          //  Debug.Log("Spawn after bool check");
           
            porContainer = Instantiate(portalParent, spawnPoint.position, Quaternion.identity);
            portalController = porContainer.GetComponent<PortalController>();
            NetworkServer.Spawn(porContainer);
            portalController.SetStoreLoc(storeLocation);
            
            for (int i = 0; i < portalCount; i++)
            {
                Vector3 spawnPointPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 2f, spawnPoint.position.z);
                GameObject portal = Instantiate(portalPrefab, spawnPointPos, Quaternion.identity, porContainer.transform);
                NetworkServer.Spawn(portal);
                // SetMaterialOnPP(portal);
                portalController.AddPuckToList(portal, i);
            }
            //  Debug.Log("End Spawn");
            //DestroyObject();
            //NetworkServer.Destroy(gameObject);
        }
    }
    

    // [ClientRpc]
    // private void SetMaterialOnPP(GameObject portal)
    // {
    //     Debug.Log(storeLocation.name);
    //     if (isServer)
    //     {
    //         portal.GetComponent<MeshRenderer>().material = player1Material;
    //     }
    //
    //     else
    //     {
    //         portal.GetComponent<MeshRenderer>().material = player2Material;
    //     }
    // }

    [Command(requiresAuthority = false)]
    public void DestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }

  

    [Server]
    public void StoreTheOtherPuck(PuckScript script)
    {
        script.ChangePosToStorage(storeLocation);
    }

    [Server]
    public void StoreTheEnemyPuck(ECscript ecscript)
    {
       // Debug.Log("Store enemy start");
        ECscript.EnemyTypes type = ecscript.enemyType;
        GameObject puck = new GameObject();
        GameObject instantiatedPuck = new GameObject();
        switch (type)
        {
            case ECscript.EnemyTypes.Goblin:
                puck = goblinPuck;
                instantiatedPuck = Instantiate(puck);
                break;
            case ECscript.EnemyTypes.Orc:
                puck = orcPuck;
                instantiatedPuck = Instantiate(puck);
                break;
            case ECscript.EnemyTypes.Ogre:
                Cmd_PortalBoolFalse();
                instantiatedPuck = gameObject;
                puck = ogrePrefab;
              //  NetworkServer.Destroy(gameObject);
                break;
            default:
               break;
        }
       // GameObject instantiatedPuck = new GameObject();
     //   instantiatedPuck = Instantiate(puck);
        if (ecscript.isLeader == true)
        {
            Cmd_SetLeaderVariant(instantiatedPuck, ecscript);
        }

        else
        {
            Cmd_PortalBoolFalse();
            Cmd_NormalVariant(instantiatedPuck);
        }

        if (instantiatedPuck != null)
        {
            NetworkServer.Spawn(instantiatedPuck);
            instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storeLocation);
            if (instantiatedPuck != gameObject)
            {
                DestroyObject();
            }
        }
        
        //Debug.Log("Store enemy end");
    }

    [Command(requiresAuthority = false)]
    private void Cmd_SetLeaderVariant(GameObject instantiatedPuck, ECscript ecscript)
    {
       Rpc_SetLeaderVariant(instantiatedPuck, ecscript);
    }

    [ClientRpc]
    private void Rpc_SetLeaderVariant(GameObject instantiatedPuck, ECscript ecscript)
    {
        ECscript.EnemyTypes type = ecscript.enemyType;
        switch (type)
        {
            case ECscript.EnemyTypes.Goblin:
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Magnet;
               
               // DestroyObject();
                break;
            case ECscript.EnemyTypes.Orc:
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Healer;
               // DestroyObject();
                break;
            case ECscript.EnemyTypes.Ogre:
                Cmd_PortalBoolFalse();
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Portal;
                break;
            default:
                break;
        }
        instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(true);
    }

    [Command(requiresAuthority = false)]
    private void Cmd_NormalVariant(GameObject instantiatedPuck)
    {
        RPC_NormalVariant(instantiatedPuck);
    }

    [ClientRpc]
    private void RPC_NormalVariant(GameObject instantiatedPuck)
    {
        instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Normal;
        instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(false);
        if (instantiatedPuck.GetComponent<PuckScript>().portalPuck != null)
        {
            instantiatedPuck.GetComponent<PuckScript>().portalPuck.canCreatePortal = false;
        }
        
        if (instantiatedPuck.GetComponent<PuckScript>().magnetPuck != null)
        {
            instantiatedPuck.GetComponent<PuckScript>().magnetPuck.canMagnet = false;
        }
    }
    
    [Command(requiresAuthority = false)]
    public void Cmd_PortalBoolFalse()
    {
     Rpc_PortalBoolFalse();   
    }

    [ClientRpc]
    private void Rpc_PortalBoolFalse()
    {
        canCreatePortal = false;
    }
}
