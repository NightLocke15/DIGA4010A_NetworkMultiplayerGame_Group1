using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoleScript : MonoBehaviour
{
    [Header("Variables")] [SerializeField] private Transform storelocation;
    
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

       if (puck.GetComponent<PuckScript>() != null)
       {
           if (puck.GetComponent<EnemyController>())
           {
               Destroy(puck.GetComponent<EnemyController>());
              // puck.tag = "Puck";
           }
         
           puck.GetComponent<PuckScript>().ChangePosToStorage(storelocation);
       }
    }
}
