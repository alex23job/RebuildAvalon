using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;
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
        /*for (i = 111; i < 117; i++)
        {
            x = i % 13; y = i / 13;
            pos.x = -12 + 2 * x + y % 2;
            pos.y = 1.5f;
            pos.z = 6.8f - 1.7f * y;
            tail = Instantiate(hexTails[3], pos, Quaternion.Euler(180f, 60 * (i - 111), 0));
        }*/
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
        if ((numberWiner > 0) && (gameUI != null)) Invoke("ViewEndPanel", 2f); 

        int numTail, rndTail = UnityEngine.Random.Range(0, 24);
        int[] nums = new int[24] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 7 };
        //int[] nums = new int[24] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 2, 3, 3 };
        numTail = nums[rndTail];
        GameObject hexTail = Instantiate(hexTails[numTail], pos, Quaternion.Euler(180f, 0, 0));
        hexTail.GetComponent<HexTail>().SetParams(numTail, this);
        return hexTail;
    }

    private void ViewEndPanel()
    {
        gameUI.ViewEndPanel(numberWiner);
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
        List<HexCandidat> listHex = new List<HexCandidat>();
        int x = 58 % 13, y = 58 / 13, ang, tx, ty, tAng, tDoor;
        foreach (GameObject hexTail in enemyWay)
        {
            HexTail hexCnt = hexTail.GetComponent<HexTail>();
            ty = Mathf.RoundToInt((6.8f - hexTail.transform.position.z) / 1.7f);
            tx = Mathf.RoundToInt((hexTail.transform.position.x + 12 - ty % 2) / 2);
            tAng = hexCnt.TailAngle; tDoor = hexCnt.TailDoors;
            ang = (360 + hexCnt.TailAngle - 180) / 60; ang %= 6;
            for (int j = 0; j < ang; j++) { tDoor = ((tDoor << 1) & 0x3f) + ((tDoor >> 5) & 1); }
            //print($"Shift tDoor => TailAngle={tAng} TailDoor={hexCnt.TailDoors}  tDoor={tDoor}");
            if ((tx < 12 && ty % 2 == 0) || (tx < 11 && ty % 2 == 1))
            {
                //if ((pole[13 * ty + tx + 1] == 0) && ((tDoor & 0x1) > 0)) listHex.Add(new HexCandidat(tx + 1, ty, 0, Mathf.Abs(ty - y) + Mathf.Abs(tx + 1 - x)));
                if ((pole[13 * ty + tx + 1] == 0) && ((tDoor & 0x1) > 0)) listHex.Add(new HexCandidat(tx + 1, ty, 0, GetDist(tx + 1, ty, x, y)));
            }
            if (tx > 0)
            {
                //if ((pole[13 * ty + tx - 1] == 0) && ((tDoor & 0x8) > 0)) listHex.Add(new HexCandidat(tx - 1, ty, 180, Mathf.Abs(ty - y) + Mathf.Abs(tx - 1 - x)));
                if ((pole[13 * ty + tx - 1] == 0) && ((tDoor & 0x8) > 0)) listHex.Add(new HexCandidat(tx - 1, ty, 180, GetDist(tx - 1, ty, x, y)));
            }
            if (ty % 2 == 0)
            {
                if (ty > 0)
                {
                    //if ((tx < 12) && (pole[13 * (ty - 1) + tx] == 0) && ((tDoor & 0x20) > 0)) listHex.Add(new HexCandidat(tx, ty - 1, 300, Mathf.Abs(ty - 1 - y) + Mathf.Abs(tx - x)));
                    if ((tx < 12) && (pole[13 * (ty - 1) + tx] == 0) && ((tDoor & 0x20) > 0)) listHex.Add(new HexCandidat(tx, ty - 1, 300, GetDist(tx, ty - 1, x, y)));
                    //if ((pole[13 * (ty - 1) + (tx - 1)] == 0) && ((tDoor & 0x10) > 0)) listHex.Add(new HexCandidat(tx - 1, ty - 1, 240, Mathf.Abs(ty - 1 - y) + Mathf.Abs(tx - 1 - x)));
                    if ((pole[13 * (ty - 1) + (tx - 1)] == 0) && ((tDoor & 0x10) > 0)) listHex.Add(new HexCandidat(tx - 1, ty - 1, 240, GetDist(tx - 1, ty - 1, x, y)));
                }
                if (ty < 8)
                {
                    //if ((tx < 12) && (pole[13 * (ty + 1) + tx] == 0) && ((tDoor & 0x2) > 0)) listHex.Add(new HexCandidat(tx, ty + 1, 60, Mathf.Abs(ty + 1 - y) + Mathf.Abs(tx - x)));
                    if ((tx < 12) && (pole[13 * (ty + 1) + tx] == 0) && ((tDoor & 0x2) > 0)) listHex.Add(new HexCandidat(tx, ty + 1, 60, GetDist(tx, ty + 1, x, y)));
                    //if ((pole[13 * (ty + 1) + (tx - 1)] == 0) && ((tDoor & 0x4) > 0)) listHex.Add(new HexCandidat(tx - 1, ty + 1, 120, Mathf.Abs(ty + 1 - y) + Mathf.Abs(tx - 1 - x)));
                    if ((pole[13 * (ty + 1) + (tx - 1)] == 0) && ((tDoor & 0x4) > 0)) listHex.Add(new HexCandidat(tx - 1, ty + 1, 120, GetDist(tx - 1, ty + 1, x, y)));
                }
            }
            if (ty % 2 == 1)
            {
                if (ty > 0 && tx < 12)
                {
                    //if ((pole[13 * (ty - 1) + tx] == 0) && ((tDoor & 0x10) > 0)) listHex.Add(new HexCandidat(tx, ty - 1, 240, Mathf.Abs(ty - 1 - y) + Mathf.Abs(tx - x)));
                    if ((pole[13 * (ty - 1) + tx] == 0) && ((tDoor & 0x10) > 0)) listHex.Add(new HexCandidat(tx, ty - 1, 240, GetDist(tx, ty - 1, x, y)));
                    //if ((pole[13 * (ty - 1) + (tx + 1)] == 0) && ((tDoor & 0x20) > 0)) listHex.Add(new HexCandidat(tx + 1, ty - 1, 300, Mathf.Abs(ty - 1 - y) + Mathf.Abs(tx + 1 - x)));
                    if ((pole[13 * (ty - 1) + (tx + 1)] == 0) && ((tDoor & 0x20) > 0)) listHex.Add(new HexCandidat(tx + 1, ty - 1, 300, GetDist(tx + 1, ty - 1, x, y)));
                }
                if (ty < 8 && tx < 12)
                {
                    //if ((pole[13 * (ty + 1) + tx] == 0) && ((tDoor & 0x4) > 0)) listHex.Add(new HexCandidat(tx, ty + 1, 120, Mathf.Abs(ty + 1 - y) + Mathf.Abs(tx - x)));
                    if ((pole[13 * (ty + 1) + tx] == 0) && ((tDoor & 0x4) > 0)) listHex.Add(new HexCandidat(tx, ty + 1, 120, GetDist(tx, ty + 1, x, y)));
                    //if ((pole[13 * (ty + 1) + (tx + 1)] == 0) && ((tDoor & 0x2) > 0)) listHex.Add(new HexCandidat(tx + 1, ty + 1, 60, Mathf.Abs(ty + 1 - y) + Mathf.Abs(tx + 1 - x)));
                    if ((pole[13 * (ty + 1) + (tx + 1)] == 0) && ((tDoor & 0x2) > 0)) listHex.Add(new HexCandidat(tx + 1, ty + 1, 60, GetDist(tx + 1, ty + 1, x, y)));
                }
            }
        }
        listHex.Sort((h1, h2) => h1.R.CompareTo(h2.R));
        foreach(HexCandidat hex in listHex)
        {
            hex.CheckHexTail(0, enemyHexs[0].GetComponent<HexTail>(), pole);
            hex.CheckHexTail(1, enemyHexs[1].GetComponent<HexTail>(), pole);
        }
        PairRotateDistance res = new PairRotateDistance(5, 720, 15);
        HexCandidat candidat = null;
        foreach (HexCandidat hex in listHex)
        {
            res = hex.GetMinDist(res, out HexCandidat hexCandidat);
            if (hexCandidat != null) { candidat = hexCandidat; }
        }
        if (res.num == 0 || res.num == 1)
        {
            HexTail enemyHexCnt = enemyHexs[res.num].GetComponent<HexTail>();
            Vector3 target = new Vector3(-12 + 2 * candidat.X + candidat.Y % 2, 1.5f, 6.8f - 1.7f * candidat.Y);
            pole[13 * candidat.Y + candidat.X] = enemyHexCnt.TailID;
            enemyHexCnt.SetTargetWay(target, res.Rot);
            enemyWay.Add(enemyHexs[res.num]);
            enemyHexs[res.num] = null;
            if (res.Dist == 0)
            {   //  enemy win ?!
                numberWiner = 2;
            }
            CreateEnemyHex();
        }

        StringBuilder sb = new StringBuilder($"res={res}  ");
        for (int i = 0; i < listHex.Count; i++)
        {
            sb.Append(listHex[i].ToString() + "  ");
        }
        print(sb.ToString());
    }

    private int GetDist(int x1, int y1, int x2, int y2)
    {
        int ry = Mathf.Abs(y1 - y2), rx = Mathf.Abs(x1 - x2);
        if ((ry > 1) && (rx > 1)) rx -= ry / 2;
        //Debug.Log($"GetDist sh={sh} x={x} dx={dx} y={y} dy={dy}  dist={Mathf.Abs(fy - dy) + Mathf.Abs(fx - dx)}");
        return ry + rx;
    }

    public void LoadCityScene()
    {
        SceneManager.LoadScene("CityScene");
    }
}

public class HexCandidat
{
    private int x, y;
    private int angle;
    private int r;
    private int numTail;
    private int tailID;
    private List<PairRotateDistance> listRes = new List<PairRotateDistance>();

    public int R { get => r; }
    public int X { get => x; }
    public int Y { get => y; }
    public int Angle { get => angle; }
    public int NumTail { get => numTail; }
    public int TailID { get => tailID; }

    public HexCandidat() { }
    public HexCandidat(int x, int y, int angle, int r)
    {
        this.x = x;
        this.y = y;
        this.angle = angle;
        this.r = r;
    }

    public void CheckHexTail(int num, HexTail hexTail, int[] pole)
    {
        int i, mask, curShift, j, d;
        for (i = 0; i < 6; i++) 
        {
            //mask = (hexTail.TailDoors >> (((720 - hexTail.TailAngle + this.angle + i * 60) / 60) % 6)) & 1;
            mask = (hexTail.TailDoors >> (((720 - hexTail.TailAngle + this.angle - i * 60) / 60) % 6)) & 1;
            if (mask > 0)
            {
                //curShift = ((720 - hexTail.TailAngle + this.angle + i * 60) / 60) % 6;
                curShift = ((720 - hexTail.TailAngle + this.angle - i * 60) / 60) % 6;
                int tDoor = hexTail.TailDoors;
                for (j = 0; j < i; j++) { tDoor = ((tDoor << 1) & 0x3f) + ((tDoor >> 5) & 1); }
                for (j = 1; j < 6; j++)
                {
                    if ((tDoor & (1 << ((curShift + j) % 6))) > 0)
                    //if ((hexTail.TailDoors & (1 << ((curShift + j) % 6))) > 0)
                    //if ((hexTail.TailDoors & (1 << ((6 + curShift - j) % 6))) > 0)
                    {   //  нашли ещё 1 выход из фишки -> надо проверить пустая ли рядом клетка и если да то вычислить расстояние и добавить пару                        
                        //d = GetDist(j, pole);
                        d = GetDist((curShift + j) % 6, pole);
                        //d = GetDist((6 + curShift - j) % 6, pole);
                        if (d >= 0)
                        {
                            int sh = i;
                            //int sh = (6 - i) % 6;
                            listRes.Add(new PairRotateDistance(num, sh * 60, d));
                            Debug.Log($"turn num={num}(d={hexTail.TailDoors}(tDoor={tDoor}) a={hexTail.TailAngle}) x={x} y={y} angle={angle} => i={i}({i * 60}) j={j}(sh={(curShift + j) % 6}) dist={d}");
                        }
                    }
                }                
            }
        }
    }

    private int GetDist(int sh, int[] pole)
    {
        int[] dj_x_0 = new int[6] { 1, 0, -1, -1, -1, 0 };
        int[] dj_x_1 = new int[6] { 1, 1, 0, -1, 0, 1 };
        int[] dj_y = new int[6] { 0, 1, 1, 0, -1, -1 };
        int dx = x + ((y % 2 == 1) ? dj_x_1[sh] : dj_x_0[sh]);
        int dy = y +  dj_y[sh];
        if ((dy < 0) || (dy > 8)) return -1;
        if (dx < 0) return -1;
        if (((dy % 2 == 0) && (dx < 13)) || ((dy % 2 == 1) && (dx < 12)))
        {
            int fx = 58 % 13, fy = 58 / 13;
            if ((fy == dy) && (fx == dx)) return 0;
            if (pole[13 * dy + dx] == 0)
            {
                int ry = Mathf.Abs(fy - dy), rx = Mathf.Abs(fx - dx);
                if ((ry > 1) && (rx > 1)) rx -= ry / 2;
                //Debug.Log($"GetDist sh={sh} x={x} dx={dx} y={y} dy={dy}  dist={Mathf.Abs(fy - dy) + Mathf.Abs(fx - dx)}");
                return ry + rx;
            }
        }
        return -1;
    }

    public PairRotateDistance GetMinDist(PairRotateDistance curMin, out HexCandidat candidat)
    {
        candidat = null;
        PairRotateDistance res = new PairRotateDistance(curMin.Number, curMin.Rot, curMin.Dist);
        foreach(PairRotateDistance pair in listRes)
        {
            if (pair.Dist < res.Dist)
            {
                res = new PairRotateDistance (pair.Number, pair.Rot, pair.Dist);
                candidat = this;
            }
        }
        return res;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (PairRotateDistance d in listRes) { sb.Append($"{d} "); }
        //return $"<{x}, {y}, {angle} gr, R={r}, num={numTail}(id={tailID}), {sb.ToString()}>";
        return $"<{x}, {y}, {angle} gr, R={r}, num={numTail}(id={tailID})>";
    }
}

public class PairRotateDistance
{
    public int num;
    private int rot;
    private int dist;

    public PairRotateDistance() { }
    public PairRotateDistance(int n, int r, int d)
    {
        num = n;
        rot = r;
        dist = d;
    }
    public override string ToString()
    {
        return $"<= num={num} r({rot}), d({dist})=>";
    }

    public int Number { get => num; }
    public int Rot { get => rot; }
    public int Dist { get => dist; }
}
