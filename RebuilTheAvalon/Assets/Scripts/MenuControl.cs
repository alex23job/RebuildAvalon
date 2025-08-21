using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

public class MenuControl : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GetLeaderboardEntries();

    //    [SerializeField] private Text[] arTxtRecItems;
    [SerializeField] private GameObject[] arRecItems;
    [SerializeField] private RawImage riAvatar;
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtRecord;

    [SerializeField] private Image imgFone;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Button btnPlay;

    private float timer = 5f;
    private bool isLoad = false;
    private int countLoad = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txtName.text = "-----";
        txtRecord.text = "-----   -----";
        ViewRecord();
        ViewLeaderboard("");
        Invoke("GetLeaderboard", 0.02f);
        //btnPlay.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        return;
        if (isLoad == false)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                imgProgress.fillAmount = (5f - timer) / 5f;
            }
            else
            {
                timer = 5f;
                countLoad--;
                if (countLoad == 0) LoadComplete();
            }
        }
    }
    public void LoadComplete()
    {
        return;
        isLoad = true;
        btnPlay.interactable = true;
        imgFone.gameObject.SetActive(false);
        imgProgress.gameObject.SetActive(false);
    }

    public void ViewAvatar()
    {
        return;
        txtName.text = GameManager.Instance.currentPlayer.playerName;
        riAvatar.texture = GameManager.Instance.currentPlayer.photo;
        Debug.Log($"ViewAvatar => name={GameManager.Instance.currentPlayer.playerName}");
        ViewRecord();
    }
    public void GetLeaderboard()
    {
#if UNITY_WEBGL
        GetLeaderboardEntries();
#endif
    }

    public void ViewRecord()
    {
        return;
        int level = GameManager.Instance.currentPlayer.maxLevel;
        if (level == 0) level = 1;
        if (Language.Instance.CurrentLanguage == "ru")
        {
            txtRecord.text = $"Óð.{level} Î÷:{GameManager.Instance.currentPlayer.totalScore} ({GameManager.Instance.currentPlayer.sessionScore})";
        }
        else
        {
            txtRecord.text = $"Lv.{level} Sc:{GameManager.Instance.currentPlayer.totalScore} ({GameManager.Instance.currentPlayer.sessionScore})";
        }
    }

    public void ViewLeaderboard(string strJson)
    {
        return;
        if (strJson == "")
        {
            Debug.Log("ViewLeaderboard strJson= <" + strJson + ">");
            for (int i = 0; i < arRecItems.Length; i++)
            {
                Text txtRecName = arRecItems[i].transform.GetChild(1).gameObject.GetComponent<Text>();
                Text txtRecScore = arRecItems[i].transform.GetChild(2).gameObject.GetComponent<Text>();
                txtRecName.text = "..............";
                txtRecScore.text = "";
            }
            return;
        }
        try
        {
            //Debug.Log("ViewLeaderboard => " + strJson);
            //PersonRecord[] data = JsonConvert.DeserializeObject<PersonRecord[]>(strJson);
            //PersonRecord[] data = JsonUtility.FromJson<PersonRecord[]>(strJson);
            PersonRecord[] data = GetDataFromJson(strJson);
            //Debug.Log("data=>" + data);
            //StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length && i < arRecItems.Length; i++)
            {
                Text txtRecName = arRecItems[i].transform.GetChild(1).gameObject.GetComponent<Text>();
                Text txtRecScore = arRecItems[i].transform.GetChild(2).gameObject.GetComponent<Text>();
                txtRecName.text = data[i].Name;
                txtRecScore.text = $"{data[i].Score}";

                //arTxtRecItems[i].text = $"{data[i]}";
                //Debug.Log("VL => " + data[i].ToString());
                //sb.Append($"{data[i]}\n");
            }
            //txtDescrLeader.text = sb.ToString();
            //Debug.Log("VL sb=" + sb.ToString());
        }
        catch
        {
            Text txtRecName = arRecItems[0].transform.GetChild(1).gameObject.GetComponent<Text>();
            txtRecName.text = Language.Instance.CurrentLanguage == "ru" ? "Îøèáêà" : "Error";
        }
        //panelLiders.SetActive(true);
    }

    private PersonRecord[] GetDataFromJson(string s)
    {
        List<PersonRecord> arr = new List<PersonRecord>();
        string[] ss = s.Split("{");
        for (int i = 1; i < ss.Length; i++)
        {
            int end = ss[i].LastIndexOf('}');
            //Debug.Log($"ss[i]={ss[i]} end={end}");
            string strJson = $"{ss[i].Substring(0, end)}";
            strJson = "{" + strJson + "}";
            //Debug.Log($"strJson={strJson}");
            PersonRecord pr = JsonUtility.FromJson<PersonRecord>(strJson);
            //Debug.Log($"pr={pr}");
            arr.Add(pr);
        }

        return arr.ToArray();
    }

    public void LoadCity()
    {
        SceneManager.LoadScene("CityScene");
    }
}

[Serializable]
public class MyArrRecords
{
    public PersonRecord[] records { get; set; }
    public MyArrRecords() { }
    public override string ToString()
    {
        return $"Counts={records.Length}";
    }
}

[Serializable]
public class PersonRecord
{
    //public int Rank { get; set; }
    public int Rank;
    //public int Score { get; set; }
    public int Score;
    //public string Name { get; set; }
    public string Name;

    public PersonRecord() { }
    public PersonRecord(int r, int sc, string nm)
    {
        Rank = r;
        Score = sc;
        Name = nm;
    }
    public override string ToString()
    {
        //string nm = String.Format("{0,-25}", Name);
        //return $"{Rank:00} {nm} {Score}";
        return $"{Rank:00} {Name} {Score}";
    }
}

