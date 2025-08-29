using NUnit.Framework;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MittenGame : MonoBehaviour
{
    [SerializeField] private MittenGameUI mittenGameUI;
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private float speedMoveBox = 5f;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject mittenPrefab;
    [SerializeField] private Material[] materials;

    private List<GameObject> boxs = new List<GameObject>();
    private List<GameObject> mittens = new List<GameObject>();
    private int countPairEnemy = 0;
    private int countPairPlayer = 0;
    private int countNewBox = 0;
    private int[] numsBox;
    private GameObject playerMitten = null;
    private GameObject enemyMitten = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mittenGameUI.ViewResult(0, 1);
        mittenGameUI.ViewResult(0, 2);
        int countBox = 9;
        numsBox = new int[countBox];
        for (int i = 0; i < countBox + 1; i++)
        {
            if (i < countBox) numsBox[i] = -1;
            AddMittenPair();
        }
        GenerateAllBoxes(countBox);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadCityScene()
    {
        SceneManager.LoadScene("CityScene");
    }

    private void AddMittenPair()
    {
        int firstColor = Random.Range(0, 4);
        int secondColor = Random.Range(0, 3);
        int threeColor = Random.Range(0, 3);
        Material[] mats = new Material[3] { materials[firstColor], materials[4 + secondColor], materials[7 + threeColor] };
        int[] numColors = new int[3] { firstColor, secondColor, threeColor};
        GameObject mitten1 = Instantiate(mittenPrefab, spawnPos, Quaternion.Euler(new Vector3(90f, 0, 0)));
        GameObject mitten2 = Instantiate(mittenPrefab, spawnPos, Quaternion.Euler(new Vector3(90f, 0, 0)));
        mitten1.GetComponent<MittenInfo>().SetColors(mats, numColors);
        mitten2.GetComponent<MittenInfo>().SetColors(mats, numColors);
        mittens.Add(mitten1);
        mittens.Add(mitten2);
    }

    private GameObject CreateNextBox()
    {
        return Instantiate(boxPrefab, spawnPos, Quaternion.identity);
    }

    private void GenerateAllBoxes(int count)
    {
        GameObject box;
        List<int> nums = new List<int>();
        int i, col1, col2;
        BoxControl boxControl;
        int povtor = 0;

        for (i = 0; i < count + 1; i++) { nums.Add(2 * i); nums.Add(2 * i + 1); }
        enemyMitten = mittens[nums[0]];
        playerMitten = mittens[nums[2]];
        enemyMitten.transform.position = new Vector3(14f, 1.5f, 0);
        playerMitten.transform.position = new Vector3(-15f, 1.5f, 0);
        nums.RemoveAt(2);nums.RemoveAt(0);
        for (i = 0; i < count; i++) 
        {
            box = CreateNextBox();
            boxControl = box.GetComponent<BoxControl>();
            boxControl.SetMittenGame(gameObject.GetComponent<MittenGame>());
            int numMitten = Random.Range(0, nums.Count);
            boxControl.SetMitten(mittens[nums[numMitten]], 0);
            col1 = mittens[nums[numMitten]].GetComponent<MittenInfo>().MittenColor;
            nums.RemoveAt(numMitten);
            povtor = 0;
            do {
                numMitten = Random.Range(0, nums.Count);
                col2 = mittens[nums[numMitten]].GetComponent<MittenInfo>().MittenColor;
                povtor++; if (povtor > 10) break;
            } while ((col1 == col2) && (nums.Count > 2));
            boxControl.SetMitten(mittens[nums[numMitten]], 1);
            nums.RemoveAt(numMitten);
            boxControl.SetParams(gameObject.GetComponent<MittenGame>(), GetPoints(new Vector3(8.5f - 8.5f * (i % 3f), 0.6f, 6.5f - 6.5f * (i / 3))), speedMoveBox, false);
            boxs.Add(box);
        }
    }

    private List<Vector3> GetPoints(Vector3 tg)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(tg);
        tg.x -= 2f;tg.y += 2f;
        points.Insert(0, tg);
        tg.x = spawnPos.x + 2;tg.y -= 1f;
        points.Insert(0, tg);
        return points;
    }

    public void SelectMitten(GameObject mitten, GameObject box)
    {
        int num = mitten.GetComponent<MittenInfo>().MittenNumber;
        BoxControl boxControl = box.GetComponent<BoxControl>();
        playerMitten = boxControl.UpdateMitten(playerMitten, num);
        if (boxControl.IsPair())
        {
            countPairPlayer++;
            mittenGameUI.ViewResult(countPairPlayer, 1);
            List<Vector3> points = new List<Vector3>();
            Vector3 pos = box.transform.position;
            AddNewBox(pos);
            pos.x += 1f; pos.y += 1f;
            points.Add(pos);
            pos.x += 5f; pos.y += 2f;
            points.Add(pos);
            pos.x = 30f;
            points.Add(pos);
            boxControl.BoxDestroy(points);
        }
    }

    private void AddNewBox(Vector3 pos)
    {
        GameObject box = CreateNextBox();
        BoxControl boxControl = box.GetComponent<BoxControl>();
        boxControl.SetMittenGame(gameObject.GetComponent<MittenGame>());
        if (countNewBox % 2 == 0)
        {
            AddMittenPair();
            AddMittenPair();
            countNewBox++;
            boxControl.SetMitten(mittens[mittens.Count - 4], 1);
            boxControl.SetMitten(mittens[mittens.Count - 2], 0);
        }
        else
        {
            countNewBox = 0;
            boxControl.SetMitten(mittens[mittens.Count - 3], 0);
            boxControl.SetMitten(mittens[mittens.Count - 1], 1);
        }
        boxControl.SetParams(gameObject.GetComponent<MittenGame>(), GetPoints(pos), speedMoveBox, true);
        boxs.Add(box);
    }

    public void DelMitten(GameObject mitten)
    {
        mittens.Remove(mitten);
    }
}
