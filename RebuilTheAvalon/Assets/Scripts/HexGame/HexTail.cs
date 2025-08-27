using UnityEngine;

public class HexTail : MonoBehaviour
{
    [SerializeField] private int tailID;
    [SerializeField] private int tailDoors;
    [SerializeField] private float speedRotation = 22.5f;
    [SerializeField] private float moveSpeed = 5f;

    private HexsGame hexsGame = null;

    public int TailID { get => tailID; }
    public int TailDoors { get => tailDoors; }
    public int TailAngle { get { return Mathf.RoundToInt(transform.rotation.eulerAngles.y); } }

    public Vector3 StartPos { get => startPos; }

    private bool isRotation = false;
    private bool isMove = false;
    private bool isMovement = false;
    private bool isWay = false;
    private Vector3 startPos;
    private Vector3 deltaPos = Vector3.zero;
    private float targetAngle = 0f;
    private Vector3 target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovement)
        {
            Vector3 delta = transform.position - target;
            if (delta.magnitude > 0.2f)
            {
                Vector3 movement = delta.normalized * moveSpeed * Time.deltaTime;
                Vector3 dm = transform.position - movement - target;
                if (dm.magnitude > 0.2f) transform.position -= movement;
                else
                {
                    transform.position = target;
                    isMovement = false;
                }
            }
            else
            {
                transform.position = target;
                isMovement = false;
            }
        }
        //if (isRotation)
        //{
        //    Vector3 rotEuler = transform.rotation.eulerAngles;
        //    float angleY = rotEuler.y;
        //    float delta = targetAngle - angleY;
        //    if (Mathf.Abs(delta) > 5f)
        //    {
        //        transform.Rotate(Vector3.up, -Time.deltaTime * speedRotation);
        //        //transform.Rotate(0, -Time.deltaTime * speedRotation, 0);
        //    }
        //    else
        //    {
        //        transform.rotation = Quaternion.Euler(180f, targetAngle, 0);
        //        isRotation = false;
        //    }
        //}
        if (isMove)
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 figPos = transform.position;
            //figPos.x += mp.x - deltaPos.x; figPos.z += 1.35f * (mp.z - deltaPos.z);
            figPos.x += mp.x - deltaPos.x; figPos.z += mp.z - deltaPos.z;
            transform.position = figPos;
            deltaPos = mp;

        }
    }

    public void SetParams(int id, HexsGame hg, bool way = false)
    {
        tailID = id;
        hexsGame = hg;
        isWay = way;
    }

    public void SetTargetWay(Vector3 tg, float rot)
    {
        //print($"SetTargetWay =>> tg={tg} rot={rot}");
        target = tg;
        //if (rot > 0) transform.Rotate(-180f, rot, 0, Space.World);
        if (rot > 0) transform.rotation = Quaternion.Euler(180f, rot, 0);
        isMovement = true;
    }

    private void OnMouseDown()
    {
        if (isWay) return;
        if (Input.GetMouseButtonDown(0))
        {
            startPos = transform.position;
            isMove = true;
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            deltaPos = mp;
        }
    }

    private void OnMouseUp()
    {
        if (isWay) return;
        if (Input.GetMouseButtonUp(0))
        { 
            if (transform.position == startPos)
            {
                //isRotation = true;
                //Vector3 rotEuler = transform.rotation.eulerAngles;
                //rotEuler.y += 60f;
                //transform.rotation = Quaternion.Euler(180f, rotEuler.y, 0);
                transform.Rotate(0, 60, 0, Space.World);
                //targetAngle += 60f;
                //print($"{transform.name} TailAngle={TailAngle} angles={transform.rotation.eulerAngles}");
                isMove = false;
                return;
            }
            if (isMove)
            {
                if (hexsGame != null)
                {
                    if (hexsGame.TestHex(gameObject, out Vector3 finalPos))
                    {
                        transform.position = finalPos;
                        isWay = true;
                    }
                    else
                    {
                        transform.position = startPos;
                    }
                }
                isMove = false; 
            }
        }
    }
}
