using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HexsGame : MonoBehaviour
{
    [SerializeField] private GameObject[] hexTails;
    [SerializeField] private GameObject playerBoard;
    [SerializeField] private GameObject enemyBoard;
    [SerializeField] private HexGameUI gameUI;

    private int numPlayerStart, numEnemyStart;
    private List<GameObject> playerWay;
    private List<GameObject> enemyWay;
    private GameObject[] playerHexs;
    private GameObject[] enemyHexs;
    private int[] pole = new int[117];
    private int numberWiner = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        playerWay = new List<GameObject>();
        enemyWay = new List<GameObject>();
        playerHexs = new GameObject[2];
        enemyHexs = new GameObject[2];
        GenerateBoard();
        CreatePlayerHex();
        CreateEnemyHex();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateBoard()
    {
        int i, x, y;
        Vector3 pos = new Vector3(0, 1, 0);
        GameObject tail;
        List<int> listPl = new List<int>();
        List<int> listEn = new List<int>();
        for (i = 0; i < 117; i++)
        {
            pole[i] = 0;
            x = i % 13; y = i / 13;
            if ((x < 2) && ((y > 0) && (y < 8))) listPl.Add(i);
            if ((y > 0) && (y < 8))
            {
                if ((y % 2 == 0) && (x > 10)) listEn.Add(i);
                if ((y % 2 == 1) && ((x > 9) && (x < 12))) listEn.Add(i);
            }            
        }
        numPlayerStart = listPl[UnityEngine.Random.Range(0, listPl.Count)];
        numEnemyStart = listEn[UnityEngine.Random.Range(0, listEn.Count)];
        print($"pl={numPlayerStart} en={numEnemyStart}");
        for (i = 0; i < 117; i++)
        {
            x = i % 13; y = i / 13;
            if ((x == 12) && (y % 2 == 1)) continue;
            pos.x = -12 + 2 * x + y % 2;
            pos.z = 6.8f - 1.7f * y;
            if (i == 58)
            {
                pole[i] = 13;
                tail = Instantiate(hexTails[10], pos, Quaternion.Euler(180f, 0, 0));
                tail.GetComponent<HexTail>().SetParams(13, this, true);
                continue;
            }
            if (i == numPlayerStart)
            {
                pole[i] = 11;
                tail = Instantiate(hexTails[8], pos, Quaternion.Euler(180f, 0, 0));
                tail.GetComponent<HexTail>().SetParams(11, this, true);
                playerWay.Add(tail);
                continue;
            }
            if (i == numEnemyStart)
            {
                pole[i] = 12;
                tail = Instantiate(hexTails[9], pos, Quaternion.Euler(180f, 0, 0));
                tail.GetComponent<HexTail>().SetParams(12, this, true);
                enemyWay.Add(tail);
                continue;
            }
            tail = Instantiate(hexTails[0], pos, Quaternion.Euler(0, 0, 0));
            tail.GetComponent<HexTail>().SetParams(0, this, true);
        }
    }

    private void CreatePlayerHex()
    {
        Vector3 pos = playerBoard.transform.position;
        pos.y += 0.5f;
        for (int i = 0; i < 2; i++)
        {
            if (playerHexs[i] == null)
            {
                pos.x = 2 * i + playerBoard.transform.position.x - 1;
                playerHexs[i] = GenerateHex(pos);
            }
        }
    }

    private void CreateEnemyHex()
    {
        Vector3 pos = enemyBoard.transform.position;
        pos.y += 0.5f;
        for (int i = 0; i < 2; i++)
        {
            if (enemyHexs[i] == null)
            {
                pos.x = 2 * i + enemyBoard.transform.position.x - 1;
                enemyHexs[i] = GenerateHex(pos);
            }
        }
    }

    private GameObject GenerateHex(Vector3 pos)
    {
        if ((numberWiner > 0) && (gameUI != null)) gameUI.ViewEndPanel(numberWiner);

        int numTail, rndTail = UnityEngine.Random.Range(0, 24);
        int[] nums = new int[24] {1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 7};
        numTail = nums[rndTail];
        GameObject hexTail = Instantiate(hexTails[numTail], pos, Quaternion.Euler(180f, 0, 0));
        hexTail.GetComponent<HexTail>().SetParams(numTail, this);
        return hexTail;
    }

    public bool TestHex(GameObject hexTail, out Vector3 finalPos)
    {
        int y = Mathf.RoundToInt((6.8f - hexTail.transform.position.z) / 1.7f);
        int x = Mathf.RoundToInt((hexTail.transform.position.x + 12 - y % 2) / 2);
        HexTail hexCnt = hexTail.GetComponent<HexTail>();
        int num = 13 * y + x;
        if (num >= 0 && num < 117 && pole[num] == 0)
        {
            if (TestConnect(x, y, hexCnt.TailAngle, hexCnt.TailID))
            {
                playerWay.Add(hexTail);
                pole[num] = hexCnt.TailID;
                finalPos = new Vector3(-12 + 2 * x + y % 2, 1.5f, 6.8f - 1.7f * y);
                Vector3 start = hexCnt.StartPos;
                int index = Mathf.RoundToInt(start.x - playerBoard.transform.position.x);
                if (index < 0) index = 0;
                playerHexs[index] = null;
                CheckingFinishWay(x, y, hexCnt.TailAngle, hexCnt.TailDoors, 1);
                CreatePlayerHex();
                MakeEnemyStep();
                return true;
            }
        }
        //print($"x={x} y={y} num={num} pole[num]={pole[num]}");
        finalPos = Vector3.zero;
        return false;
    }

    private bool TestConnect(int tx, int ty, int angle, int tailID)
    {
        int[] doors = new int[8] {0, 9, 3, 5, 13, 11, 21, 63};
        int x, y, num = 13 * ty + tx;
        foreach(GameObject tail in playerWay)
        {
            HexTail hexTail = tail.GetComponent<HexTail>();
            y = Mathf.RoundToInt((6.8f - tail.transform.position.z) / 1.7f);
            x = Mathf.RoundToInt((tail.transform.position.x + 12 - y % 2) / 2);
            Vector3 tv = new Vector3(-12 + 2 * tx + ty % 2, 0, 6.8f - 1.7f * ty);
            Vector3 cur = new Vector3(tail.transform.position.x, 0, tail.transform.position.z);
            //print($"dv={Vector3.Angle(Vector3.right, tv - cur)}");
            int dv = Mathf.RoundToInt(Vector3.Angle(Vector3.right, tv - cur));
            if (y == ty + 1) dv = 360 - dv;
            //print($"TestConnect x={x} y={y} tailNum={13*y+x} num={num} angle={angle} tailID={tailID} doors={doors[tailID]} curAngle={hexTail.TailAngle} curID={hexTail.TailID} curDoor={hexTail.TailDoors}  dv={0}");
            //return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv);
            if ((y == ty) && ((x == tx - 1) || (x == tx + 1)))
            {
                if (CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv)) return true;
            }
            if ((y == ty + 1) && ((x == tx) || (((x == tx - 1) && (y % 2 == 1)) || ((x == tx + 1) && (y % 2 == 0)))))
            {
                if (CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv)) return true;
            }
            if ((y == ty - 1) && ((x == tx) || (((x == tx - 1) && (y % 2 == 1)) || ((x == tx + 1) && (y % 2 == 0)))))
            {
                if (CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv)) return true;
            }
            /*if (y == ty)
            {
                if (x == tx - 1)
                {
                    //print($"dv={180}");
                    dv = 180;
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv);
                    //return true;
                }
                if (x == tx + 1)
                {
                    //print($"dv={0}");
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv);
                    //return true;
                }
            }
            if (y == ty + 1)
            {
                if (x == tx)
                {
                    if (y % 2 == 1) dv = 60; // print($"dv={60}");
                    if (y % 2 == 0) dv = 120; // print($"dv={120}");
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv);
                    //return true;
                }
                if (((x == tx - 1) && (y % 2 == 1)) || ((x == tx + 1) && (y % 2 == 0))) 
                {
                    if ((x == tx - 1) && (y % 2 == 1)) dv = 120;    // print($"dv={120}");
                    if ((x == tx + 1) && (y % 2 == 0)) dv = 60; // print($"dv={60}");
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv); 
                    //return true;
                }
            }
            if (y == ty - 1)
            {
                if (x == tx)
                {
                    if (y % 2 == 1) dv = 300;   // print($"dv={300}");
                    if (y % 2 == 0) dv = 240;   // print($"dv={240}");
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv); 
                    //return true;
                }
                if (((x == tx - 1) && (y % 2 == 1)) || ((x == tx + 1) && (y % 2 == 0)))
                {
                    if ((x == tx - 1) && (y % 2 == 1)) dv = 240;    // print($"dv={240}");
                    if ((x == tx + 1) && (y % 2 == 0)) dv = 300;    // print($"dv={300}");
                    return CheckingConnectivity(angle, doors[tailID], hexTail.TailAngle, hexTail.TailDoors, dv); 
                    //return true;
                }
            }*/
        }
        return false;
    }

    private bool CheckingConnectivity(int ang1, int door1, int ang2, int door2, int dv)
    {
        //  ang1 и door1 - пристыковываемый hex
        //  ang2 и door2 - hex, к которому пробуем стыковать 
        //int mask1 = (door1 >> (((ang1 + dv) / 60) % 6)) & 1;
        //int mask2 = (door2 >> (((ang2 + dv + 180) / 60) % 6)) & 1;
        int mask1 = (door1 >> (((720 - ang1 + dv) / 60) % 6)) & 1;
        int mask2 = (door2 >> (((720 - ang2 + dv + 180) / 60) % 6)) & 1;
        print($"a1={ang1} d1={door1} a2={ang2} d2={door2} dv={dv} m1={mask1} m2={mask2}");
        //if (mask1 == mask2) return true;
        if ((mask1 == 1) && (mask2 == 1)) return true;
        return false;
    }

    private void CheckingFinishWay(int x, int y, int angle, int door, int numWin)
    {
        int fx = 58 % 13, fy = 58 / 13;
        int dv = GetDeltaAngle(x, y, fx, fy);
        int mask = -1;
        //if ((fy == y) && ((fx == x + 1) || (fx == x - 1))) mask = (door >> (((720 + angle - dv + 180) / 60) % 6)) & 1;
        //else if ((y == fy + 1) && ((x == fx) || (((x == fx - 1) && (y % 2 == 1)) || ((x == fx + 1) && (y % 2 == 0))))) mask = (door >> (((720 + angle - dv + 180) / 60) % 6)) & 1;
        //else if ((y == fy - 1) && ((x == fx) || (((x == fx - 1) && (y % 2 == 1)) || ((x == fx + 1) && (y % 2 == 0))))) mask = (door >> (((720 + angle - dv + 180) / 60) % 6)) & 1;
        if ((fy == y) && ((fx == x + 1) || (fx == x - 1))) mask = (door >> (((720 - angle + dv) / 60) % 6)) & 1;
        else if ((y == fy + 1) && ((x == fx) || (((x == fx - 1) && (y % 2 == 1)) || ((x == fx + 1) && (y % 2 == 0))))) mask = (door >> (((720 - angle + dv) / 60) % 6)) & 1;
        else if ((y == fy - 1) && ((x == fx) || (((x == fx - 1) && (y % 2 == 1)) || ((x == fx + 1) && (y % 2 == 0))))) mask = (door >> (((720 - angle + dv) / 60) % 6)) & 1;
        print($"CheckingFinishWay x={x} y={y} angle={angle} door={door} dv={dv} mask={mask}");
        if (mask > 0) numberWiner = numWin;
    }

    private int GetDeltaAngle(float x1, float z1, float x2, float z2)
    {
        Vector3 tv = new Vector3(-12 + 2 * x1 + z1 % 2, 0, 6.8f - 1.7f * z1);
        Vector3 cur = new Vector3(-12 + 2 * x2 + z2 % 2, 0, 6.8f - 1.7f * z2);
        //print($"dv={Vector3.Angle(Vector3.right, tv - cur)}");
        int dv = Mathf.RoundToInt(Vector3.Angle(Vector3.right, tv - cur));
        if (z2 == z1 + 1) dv = 360 - dv;
        return dv;
    }

    private void MakeEnemyStep()
    {

    }

    public void LoadCityScene()
    {
        SceneManager.LoadScene("CityScene");
    }
}
