using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private bool move;
    [SerializeField] private float moveTime;

    private void Start()
    {
        target = GameObject.Find("Tower");
    }

    private void Update()
    {
        if (move == true)
        {
            moveTime += Time.deltaTime;
            EnemyMove();
        }

        if (moveTime > 0.5f)
        {
            move = false;
            moveTime = 0f;
        }
    }

    private void EnemyMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 2 * Time.deltaTime);
    }
}
