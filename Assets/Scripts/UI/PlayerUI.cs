using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider towerHealthSlider;
    private TowerHandler towerHandler;

    private void Start()
    {
        towerHealthSlider = transform.GetChild(1).gameObject.GetComponent<Slider>();
        towerHandler = GameObject.Find("Tower").GetComponent<TowerHandler>();
    }

    [ClientCallback]
    private void Update()
    {
        towerHealthSlider.value = towerHandler.towerHealth;
    }
}
