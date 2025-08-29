using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BoxControl : MonoBehaviour
{
    private MittenGame mittenGame = null;
    //private float timer = 5f;
    private bool isMove = false;
    private float moveSpeed = 5.0f;
    private Animator anim;
    private List<Vector3> points = new List<Vector3>();
    private Vector3 target;
    private GameObject[] mittens = new GameObject[2];
    private BoxCollider boxCollider = null;

    private void Awake()
    {
        anim = transform.GetChild(1).gameObject.GetComponent<Animator>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

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
        if (points.Count > 0)
        {
            target = points[0];
            points.RemoveAt(0);
        }
        else isMove = false;
    }

    private void OnMouseUp()
    {
        anim.SetTrigger("TriggerOpen");
        Invoke("MittensUp", 1f);
        Invoke("MittensDown", 5f);
    }

    private void MittensUp()
    {
        for (int i = 0; i < mittens.Length; i++)
        {
            mittens[i].GetComponent<MittenInfo>().MittenUp(true);
        }
        boxCollider.isTrigger = true;
    }

    private void MittensDown()
    {
        for (int i = 0; i < mittens.Length; i++)
        {
            mittens[i].GetComponent<MittenInfo>().MittenUp(false);
        }
        boxCollider.isTrigger = false;
    }

    public void SetMittenGame(MittenGame mg)
    {
        mittenGame = mg;
    }

    public void SetParams(MittenGame mg, List<Vector3> pt, float speed, bool isDelay)
    {
        //print($"BoxControl SetParams mg=<{mg}>");
        moveSpeed = speed;
        mittenGame = mg;
        points.Clear();
        for (int i = 0; i < pt.Count; i++)
        {
            points.Add(new Vector3(pt[i].x, pt[i].y, pt[i].z));
        }
        target = points[0];
        points.RemoveAt(0);
        Invoke("StartMoving", isDelay ? 4f : 1f);
    }

    private void StartMoving()
    {
        isMove = true;
    }

    public void SetMitten(GameObject mitten, int num)
    {
        mittens[num] = mitten;
        mitten.transform.parent = transform;
        mitten.transform.localPosition = new Vector3(-2f + 3.5f * num, 0.15f, 0);
        mitten.GetComponent<MittenInfo>().SetNumber(num, mittenGame);
    }

    public GameObject GetMitten(int num)
    {
        return mittens[num];
    }

    public GameObject UpdateMitten(GameObject mitten, int num)
    {
        GameObject res = mittens[num];
        MittenInfo info = res.GetComponent<MittenInfo>();
        info.SetNumber(num, mittenGame);
        info.SetTarget(mitten.transform.position, moveSpeed);
        info.transform.parent = null;
        SetMitten(mitten, num);
        return res;
    }

    public bool IsPair()
    {
        return mittens[0].GetComponent<MittenInfo>().MittenColor == mittens[1].GetComponent<MittenInfo>().MittenColor;
    }

    public void BoxDestroy(List<Vector3> pt)
    {
        for (int i = 0; i < pt.Count; i++)
        {
            points.Add(new Vector3(pt[i].x, pt[i].y, pt[i].z));
        }
        target = points[0];
        points.RemoveAt(0);        
        mittenGame.DelMitten(mittens[0]);
        mittenGame.DelMitten(mittens[1]);
        Invoke("MoveAndDestroy", 4f);
    }

    private void MoveAndDestroy()
    {
        isMove = true;
        Destroy(gameObject, 5f);
    }
}
