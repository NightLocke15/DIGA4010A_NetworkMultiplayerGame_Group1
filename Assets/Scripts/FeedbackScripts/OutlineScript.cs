using UnityEngine;

public class OutlineScript : MonoBehaviour
{
    [SerializeField] private Outline outline;
    [SerializeField] private PuckScript puckScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (puckScript.canDrag)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }
}
