using UnityEngine;
using UnityEngine.SceneManagement;

public class Offline : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(GameObject.Find("NetworkManager"));
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
