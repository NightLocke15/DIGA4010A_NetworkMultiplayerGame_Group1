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
           ECscript.EnemyTypes enemyType = puck.GetComponent<ECscript>().enemyType;
           puck.GetComponent<ECscript>().DeleteStuff();
           CmdSpawn(enemyType);
           return;
       }
       
       
       if (puck.GetComponent<PuckScript>() != null)
       {
           puck.GetComponentInChildren<PuckScript>().ChangePosToStorage(storelocation);
       }
    }

    [Command(requiresAuthority = false)]
    private void CmdSpawn(ECscript.EnemyTypes type)
    {
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
        instantiatedPuck.GetComponent<PuckScript>().ChangePosToStorage(storelocation);
    }
}
