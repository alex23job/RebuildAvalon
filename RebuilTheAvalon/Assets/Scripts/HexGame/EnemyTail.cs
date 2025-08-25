using UnityEngine;

public class EnemyTail : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    private Vector3 target;
    private bool isMove = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            Vector3 delta = target - transform.position;
            if (delta.magnitude > 0.5f)
            {
                Vector3 movement = delta.normalized * moveSpeed * Time.deltaTime;
                Vector3 currentPos = transform.position;
                Vector3 nd = target - (currentPos + movement);
                if (nd.magnitude <= 0.5f)
                {
                    transform.position = target;
                    isMove = false;
                }
                else
                {
                    currentPos += movement;
                    transform.position = currentPos;
                }
            }
            else
            {
                transform.position = target;
                isMove = false;
            }
        }
    }

    public void SetTarget(Vector3 tg)
    {
        target = tg;
        isMove = true;
    }
}
