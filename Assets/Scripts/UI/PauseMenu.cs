using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject disconnectedPanel;
    public bool isPaused;
    [SerializeField] private CustomNetworkManager manager;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    [Command (requiresAuthority = false)]
    public void CmdPauseGame()
    {
        RpcPauseGame();
    }

    [ClientRpc]
    public void RpcPauseGame()
    {
        Debug.Log("paused");
        isPaused = true;
        pausePanel.SetActive(true);
    }

    [Command(requiresAuthority = false)]
    public void CmdResumeGame()
    {
        RpcResumeGame();
    }

    [ClientRpc]
    public void RpcResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    [Command (requiresAuthority =false)] 
    public void CmdMenu()
    {
        disconnectedPanel.SetActive(false);
        RpcMenu();
    }

    [ClientRpc]
    public void RpcMenu()
    {
        manager.menu = true;
        disconnectedPanel.SetActive(false);
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StopHost();
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StopClient();
        SceneManager.LoadScene(1);
    }
}
