using System;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoleScript : NetworkBehaviour
{
    [Header("Variables")] [SerializeField] private Transform storelocation;

    [SerializeField] private GameObject goblinPuck, orcPuck, ogrePuck;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
       GameObject puck = other.gameObject;

       if (puck.GetComponent<ECscript>())
       {
           //ECscript.EnemyTypes enemyType = puck.GetComponent<ECscript>().enemyType;
           CmdSpawn(puck.GetComponent<ECscript>());
           puck.GetComponent<ECscript>().DeleteStuff();
           return;
       }
       
       
       if (puck.GetComponent<PuckScript>() != null)
       {
            puck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Normal;
            //puck.GetComponentInChildren<PuckScript>().leaderCircle.SetActive(false);
            puck.GetComponentInChildren<PuckScript>().ChangePosToStorage(storelocation); 
            
            //puck.transform.GetChild(2).gameObject.SetActive(false);
       }
    }

    [Command(requiresAuthority = false)]
    private void CmdSpawn(ECscript ecscript)
    {
      //  Debug.Log(ecscript.gameObject.name);
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
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        GameObject instantiatedPuck = Instantiate(puck);
        NetworkServer.Spawn(instantiatedPuck);
        if (ecscript.isLeader == true)
        {
           SetVarient(instantiatedPuck, ecscript);
        }

        else
        {
            SetNormalH(instantiatedPuck, ecscript);
        }
        
        
        instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storelocation);
    }

    [ClientRpc]
    private void SetVarient(GameObject instantiatedPuck, ECscript ecscript)
    {
        instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(false);
        ECscript.EnemyTypes type = ecscript.enemyType;
        switch (type)
        {
            case ECscript.EnemyTypes.Goblin:
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Magnet;
                instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(true);
                instantiatedPuck.GetComponent<PuckScript>().magnetPuck.Cmd_deActivateMag();
                break;
            case ECscript.EnemyTypes.Orc:
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Healer;
                instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(true);
                break;
            case ECscript.EnemyTypes.Ogre:
                instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Portal;
                instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [ClientRpc]
    private void SetNormalH(GameObject instantiatedPuck, ECscript ecscript)
    {
        instantiatedPuck.GetComponent<PuckScript>().variant = PuckScript.puckVariants.Normal;
       // instantiatedPuck.GetComponent<PuckScript>().leaderCircle.SetActive(false);
    }
}
