using UnityEngine;
using Mirror;
public class SceneChanger : MonoBehaviour
{
    // This should be called when the UI Button is pressed
    public void ChangeScene()
    {
        // Only the server (host) is allowed to change the scene
        if (NetworkServer.active)
        {
            // Change scene for everyone
            NetworkManager.singleton.ServerChangeScene("Example");
        }
    }
}
