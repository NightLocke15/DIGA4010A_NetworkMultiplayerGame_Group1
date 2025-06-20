using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject disconnectedPanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject howToPlayPanel;
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
        pauseButton.SetActive(false);
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
        pauseButton.SetActive(true);
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

    public void HowToPlay()
    {
        howToPlayPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void Back()
    {
        howToPlayPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
