using kcp2k;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NUnit.Framework;

public class MenuUI : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject lobbyPanel;
    public GameObject tutorialPanel;
    public GameObject waitingPanel;
    public GameObject startButton;
    public GameObject endScreenPanel;
    public EnemySpawning enemySpawningScript;

    public NetworkManager networkManager;
    public KcpTransport kcpTransport;

    public TMP_InputField ipAddress;
    public TMP_InputField portAddress;

    private void Start()
    {
        lobbyPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        waitingPanel.SetActive(false);
        startButton.SetActive(false);
        endScreenPanel.SetActive(false);
    }

    private void Update()
    {

    }

    public void OnClickPlay()
    {
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void OnClickBack()
    {
        lobbyPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnClickTutorial()
    {
        menuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickHost()
    {
        NetworkAddress();
        networkManager.StartHost();
        Connected();
    }

    public void OnClickServer()
    {
        NetworkAddress();
        networkManager.StartServer();
        Connected();
    }

    public void OnClickClient()
    {
        NetworkAddress();
        networkManager.StartClient();
        Connected();
    }

    public void NetworkAddress()
    {
        if (!string.IsNullOrEmpty(ipAddress.text))
        {
            networkManager.networkAddress = ipAddress.text;
        }

        if (ushort.TryParse(portAddress.text, out ushort port))
        {
            kcpTransport.port = port;
        }
    }

    public void Connected()
    {
        lobbyPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }

    
    
}
