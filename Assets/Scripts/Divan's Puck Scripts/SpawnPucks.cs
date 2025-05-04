using Mirror;
using UnityEngine;

public class SpawnPucks : NetworkBehaviour
{
    [SerializeField] private GameObject puck;

    public override void OnStartServer()
    {
        GameObject newPuck = Instantiate(puck, puck.transform.position, puck.transform.rotation);
        NetworkServer.Spawn(newPuck);
    }

    //void Update()
    //{
    //    if (GameObject.Find("PlayerOne(Clone)")  != null && GameObject.Find("PlayerTwo(Clone)") != null)
    //    {
    //        if (Input.GetKeyDown(KeyCode.P))
    //        {
    //            for (int i = 0; i < 4; i++)
    //            {
    //                GameObject newPuck = Instantiate(puck, GameObject.Find("PlayerOne(Clone)").transform.GetChild(4));
    //            }   

    //            for (int i = 0;i < 4;i++)
    //            {
    //                GameObject newPuck = Instantiate(puck, GameObject.Find("PlayerTwo(Clone)").transform.GetChild(4));
    //            }
    //        }
    //    }
    //}
}
