using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject menuPanel;
    public bool isPaused;

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
        RpcMenu();
    }

    [ClientRpc]
    public void RpcMenu()
    {
        SceneManager.LoadScene(0);
        NetworkClient.Disconnect();
        NetworkServer.DisconnectAll();
        
    }
}
