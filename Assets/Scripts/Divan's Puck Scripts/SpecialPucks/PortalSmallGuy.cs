using System;
using UnityEngine;

public class PortalSmallGuy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Puck")
        {
            Debug.Log(collision.gameObject.name);
            //StoreThePuck(collision.gameObject.GetComponent<PuckScript>());
            collision.gameObject.GetComponentInChildren<PuckScript>().ChangePosToStorage(gameObject.GetComponentInParent<PortalController>().storeLocation);
            gameObject.GetComponentInParent<PortalController>().RemovePuckFromList();
        }
        
        else if (collision.gameObject.tag == "Enemy")
        {
            ECscript eCscript = collision.gameObject.GetComponentInChildren<ECscript>();
            gameObject.GetComponentInParent<PortalController>().StoreTheEnemyPuck(eCscript);
            //eCscript.DeleteStuff();
            gameObject.GetComponentInParent<PortalController>().RemovePuckFromList();
        }
    }
}
