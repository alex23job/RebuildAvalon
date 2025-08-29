using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MittenInfo : MonoBehaviour
{
    private int firstColorNumber = 0;
    private int secondColorNumber = 0;
    private int threeColorNumber = 0;
    private int mittenNumber = -1;
    private MittenGame mittenGame = null;
    private bool isMove = false;
    private Vector3 target;
    private float moveSpeed = 10f;
    private bool isUpped = false;

    public int MittenNumber { get => mittenNumber; }
    public int MittenColor {  get { return 100 * firstColorNumber + 10 * secondColorNumber + threeColorNumber; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMove)
        {
            Vector3 delta = transform.position - target;
            if (delta.magnitude > 0.5f)
            {
                Vector3 movement = delta.normalized * moveSpeed * Time.deltaTime;
                Vector3 dm = transform.position - movement - target;
                if (dm.magnitude > 0.2f) transform.position -= movement;
                else
                {
                    EndMove();
                }
            }
            else
            {
                EndMove();
            }
        }
    }

    private void EndMove()
    {
        transform.position = target;
        isMove = false;
    }

    public void SetColors(Material[] mats, int[] numColors)
    {
        //mittenNumber = num;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        Material[] mittenMats = mr.materials;

        for (int i = 0; i < mats.Length; i++)
        {
            switch(i)
            {
                case 0:
                    mittenMats[0] = mats[0];
                    firstColorNumber = numColors[0];
                    break;
                case 1:
                    mittenMats[2] = mats[1];
                    if (numColors.Length > 1) secondColorNumber = numColors[1];
                    break;
                case 2:
                    mittenMats[3] = mats[2];
                    if (numColors.Length > 2) threeColorNumber = numColors[2];
                    break;
            }
        }

        mr.materials = mittenMats;
    }

    private void OnMouseUp()
    {
        //print($"{gameObject.name}(col={MittenColor} num={mittenNumber}) {((transform.parent != null) ? transform.parent.gameObject.name : null)} mg={mittenGame}");
        if (transform.parent != null && mittenGame != null)
        {
            //print($" in if {gameObject.name} {transform.parent.gameObject.name}");
            mittenGame.SelectMitten(gameObject, transform.parent.gameObject);
        }
    }

    public void SetNumber(int num, MittenGame mg)
    {
        mittenNumber = num;
        mittenGame = mg;
    }

    public void SetTarget(Vector3 tg, float speed)
    {
        moveSpeed = speed;
        target = tg;
        isMove = true;
        isUpped = false;
    }

    public void MittenUp(bool isUp)
    {
        Vector3 pos = transform.localPosition;
        if (isUp)
        {
            pos.y += 1f;
            if (isUpped == false) transform.localPosition = pos;
            isUpped = true;
        }
        else
        {
            pos.y -= 1f;
            if (isUpped) transform.localPosition = pos;
            isUpped = false;
        }
    }
}
