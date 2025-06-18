using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    private TurnOrderManager turnOrderManager;
    private TowerHandler towerHandler;
    [SerializeField] private GameObject playerOneTurnImg;
    [SerializeField] private GameObject playerTwoTurnImg;
    [SerializeField] private GameObject enemyTurnImg;
    [SerializeField] private GameObject yourTurnImg;
    [SerializeField] private TextMeshProUGUI wave;

    private void Start()
    {
       // towerHealthSlider = transform.GetChild(0).gameObject.GetComponent<Slider>();
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
        turnOrderManager = GameObject.Find("Manager").GetComponent<TurnOrderManager>();
        playerOneTurnImg.SetActive(false);
        playerTwoTurnImg.SetActive(false);
        enemyTurnImg.SetActive(false);
        yourTurnImg.SetActive(false);
    }

    [ClientCallback]
    private void Update()
    {
        if (isServer)
        {
           if (turnOrderManager.playerOneText == "Enemy Turn")
           {
                enemyTurnImg.SetActive(true);
                yourTurnImg.SetActive(false);
                playerOneTurnImg.SetActive(false);
                playerTwoTurnImg.SetActive(false);
           }
           else if (turnOrderManager.playerOneText == "Your Turn!")
           {
                enemyTurnImg.SetActive(false);
                yourTurnImg.SetActive(true);
                playerOneTurnImg.SetActive(false);
                playerTwoTurnImg.SetActive(false);
           }
           else if (turnOrderManager.playerOneText == "Player Two Turn")
           {
                enemyTurnImg.SetActive(false);
                yourTurnImg.SetActive(false);
                playerOneTurnImg.SetActive(false);
                playerTwoTurnImg.SetActive(true);
           }
        }
        else
        {
            if (turnOrderManager.playerTwoText == "Enemy Turn")
            {
                enemyTurnImg.SetActive(true);
                yourTurnImg.SetActive(false);
                playerOneTurnImg.SetActive(false);
                playerTwoTurnImg.SetActive(false);
            }
            else if (turnOrderManager.playerTwoText == "Your Turn!")
            {
                enemyTurnImg.SetActive(false);
                yourTurnImg.SetActive(true);
                playerOneTurnImg.SetActive(false);
                playerTwoTurnImg.SetActive(false);
            }
            else if (turnOrderManager.playerTwoText == "Player One Turn")
            {
                enemyTurnImg.SetActive(false);
                yourTurnImg.SetActive(false);
                playerOneTurnImg.SetActive(true);
                playerTwoTurnImg.SetActive(false);
            }
        }

        wave.text = turnOrderManager.totalWaves.ToString();
    }
}
