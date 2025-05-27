using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private Slider towerHealthSlider;
    private TurnOrderManager turnOrderManager;
    private TowerHandler towerHandler;
    [SerializeField] private TextMeshProUGUI playerText;

    private void Start()
    {
       // towerHealthSlider = transform.GetChild(0).gameObject.GetComponent<Slider>();
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
        turnOrderManager = GameObject.Find("Manager").GetComponent<TurnOrderManager>();
    }

    [ClientCallback]
    private void Update()
    {
        towerHealthSlider.value = towerHandler.towerHealth;
        if (isServer)
        {
            playerText.text = turnOrderManager.playerOneText;
        }
        else
        {
            playerText.text = turnOrderManager.playerTwoText;
        }
    }
}
