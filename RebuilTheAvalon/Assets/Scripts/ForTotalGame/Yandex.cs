using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class Yandex : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern bool HasFocus();

    [DllImport("__Internal")]
    private static extern void GamePlayReady();

    [DllImport("__Internal")]
    private static extern void GamePlayStart();

    [DllImport("__Internal")]
    private static extern void GamePlayStop();

    [DllImport("__Internal")]
    private static extern void GetPlayerData();

    [DllImport("__Internal")]
    private static extern void RateGame();

    [DllImport("__Internal")]
    private static extern void AddLiveExtern(int value);

    [DllImport("__Internal")]
    private static extern void AddBonusExtern(int value);

    /*
    [SerializeField] private Text txtName;
    [SerializeField] private RawImage photo;
    [SerializeField] private Text txtInfo;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtPlay;
    [SerializeField] private Text txtQuit;
    [SerializeField] private Button btnShowAdw;
    [SerializeField] private AudioSource soundFone;    
    */
    [SerializeField] private GameObject authPanel;
    [SerializeField] private Text authDescr;
    [SerializeField] private Text authBtnText;
    [SerializeField] private MenuControl startMenu;
    [SerializeField] private LevelControl levelControl;

    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        /*if (txtName != null)
        {
            txtName.text = "-----";
        }*/
        Invoke("Zapros", 0.05f);
    }

    private void Zapros()
    {
#if UNITY_WEBGL
        GetPlayerData();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //Invoke("TestProgressBar", 10f);
    }

    private void TestProgressBar()
    {
        if (startMenu != null) startMenu.LoadComplete();
    }

    public void ClickRateButton()
    {
        RateGame();
    }

    public void ClickAdwButton()
    {
        if (levelControl != null)
        {
            //levelControl.SoundPause();
            //levelControl.ButtonAdvInteractable(false);
        }
        //soundFone.Pause();
        //btnShowAdw.gameObject.SetActive(false);
        AddLiveExtern(1);
        GameStop();
    }
    public void ClickRewardButton()
    {
        if (levelControl != null)
        {
            //levelControl.SoundPause();
            //levelControl.UpdateBtnAdw(false);
        }
        //soundFone.Pause();
        //btnShowAdw.gameObject.SetActive(false);

        //AddRewardBonus(3);
        
        AddBonusExtern(1);
        GameStop();
    }

    public void AddLiveAdw(int value)
    {
        //GameManager.Instance.currentPlayer.AddLive();
        //PlayerInfo.Instance.countLive++;
        //SaveGame();
        Invoke("SoundPlay", 5f);
    }

    public void AddRewardBonus(int value)
    {
        //GameManager.Instance.currentPlayer.UpdateReward(GameManager.Instance.currentPlayer.currentLevel - 1);
        //GameManager.Instance.SaveGame();
        GameStart();
        //levelControl.ClearThreeLines();
        //levelControl.ChangeSimpleFigure();
        
        //levelControl.Generate3AdsTail(value);
        
        Invoke("SoundPlay", 5f);
    }

    public void AdvRewardedClose()
    {
        GameStart();
    }

    public void ViewAuthPanel()
    {
        Invoke("AuthPanelView", 0.5f);
    }

    private void AuthPanelView()
    {
        if (authPanel != null)
        {
            authBtnText.GetComponent<InterText>().UpdateLanguage();
            authDescr.GetComponent<InterText>().UpdateLanguage();
            authPanel.SetActive(true);
        }
    }

    public void CloseAdw()
    {
        //Invoke("SoundPlay", 5f);
    }

    private void SoundPlay()
    {
        //if (levelControl != null) levelControl.SoundPlay();
    }

    public void SetName(string name)
    {
        string[] nm = name.Split(' ');
        if (startMenu != null) GameManager.Instance.currentPlayer.playerName = nm[0];
        Debug.Log($"SetName => n={nm.Length}  name={GameManager.Instance.currentPlayer.playerName}");
        if (startMenu != null)
        {
            startMenu.ViewAvatar();
            if (startMenu != null) startMenu.LoadComplete();
        }
        /*if (txtName != null)
        {
            txtName.text = name;
        }*/
    }

    public void SetPhoto(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    IEnumerator DownloadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            GameManager.Instance.currentPlayer.photo = ((DownloadHandlerTexture)request.downloadHandler).texture;
            if (startMenu != null)
            {
                startMenu.ViewAvatar();
            }
        }
    }

    public void TranslateLiderboardEntries(string strJson)
    {
        Debug.Log($"TranslateLiderboardEntries isReady = {isReady}");
        if (isReady)
        {
            GamePlayReady();
            if (startMenu != null) startMenu.LoadComplete();
        }
        else isReady = true;
        if (startMenu != null) startMenu.ViewLeaderboard(strJson);
    }

    public void GameStart()
    {
#if UNITY_WEBGL
        GamePlayStart();
#endif
    }

    public void GameStop()
    {
#if UNITY_WEBGL
        GamePlayStop();
#endif
    }

    public bool IsFocus()
    {
#if UNITY_WEBGL
        return HasFocus();
#endif
        return true;
    }

    /*private void SaveGame()
    {
        SaveData data = new SaveData();
        data.level = PlayerInfo.Instance.currentLevel;
        data.maxLevel = PlayerInfo.Instance.maxLevel;
        data.countLive = PlayerInfo.Instance.countLive;
        data.score = PlayerInfo.Instance.totalScore;
        DateTime dt = DateTime.Now;
        data.timeString = $"{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}-{dt.Hour:00}";
        string jsonStr = JsonUtility.ToJson(data);
        PlayerInfo.Instance.Save(jsonStr);
    }*/

    public void SetPlayerInfo(string jsonStr)
    {
        Debug.Log($"SetPlayerInfo isReady = {isReady}");
        if (isReady)
        {
            GamePlayReady();
        }
        else isReady = true;

        //Debug.Log($"PlayerInfoJsonString => {jsonStr}");
        SaveData data = JsonUtility.FromJson<SaveData>(jsonStr);
        if (startMenu != null) GameManager.Instance.UpdateLoadingData(data);
        if (startMenu != null) startMenu.ViewAvatar();
        /*PlayerInfo.Instance.currentLevel = data.level;
        PlayerInfo.Instance.maxLevel = data.maxLevel;
        PlayerInfo.Instance.countLive = data.countLive;
        PlayerInfo.Instance.totalScore = data.score;
        if (PlayerInfo.Instance.countLive < 5)
        {
            string[] arrData = data.timeString.Split('-');
            if (arrData.Length == 4)
            {
                if (int.TryParse(arrData[0], out int year) && int.TryParse(arrData[1], out int month) && int.TryParse(arrData[2], out int day) && int.TryParse(arrData[3], out int hour))
                {
                    DateTime oldDt = new DateTime(year, month, day, hour, 0, 0);
                    DateTime dt = DateTime.Now;
                    TimeSpan delta = dt - oldDt;
                    int countDeltaLive = (int)delta.TotalHours;
                    if (countDeltaLive > 0)
                    {
                        PlayerInfo.Instance.countLive += countDeltaLive;
                        if (PlayerInfo.Instance.countLive > 5) PlayerInfo.Instance.countLive = 5;
                    }
                }
            }
        }
        if (PlayerInfo.Instance.maxLevel == 0 && PlayerInfo.Instance.currentLevel == 0) PlayerInfo.Instance.countLive = 5;
        if (PlayerInfo.Instance.currentLevel == 0) PlayerInfo.Instance.currentLevel = 1;
        if (PlayerInfo.Instance.maxLevel == 0) PlayerInfo.Instance.maxLevel = 1;
        if (txtInfo != null)
        {
            if (Language.Instance.CurrentLanguage == "ru")
            {
                string so = PlayerInfo.Instance.currentLevel == 2 ? "о" : "";
                string s1 = $"Число доступных попыток : {PlayerInfo.Instance.countLive}";
                string s2 = $"Число набранных очков : {PlayerInfo.Instance.totalScore}";
                string s3 = $"Продолжить с{so} {PlayerInfo.Instance.currentLevel} уровня";
                txtInfo.text = $"{s1}\n{s2}\n{s3}";
                txtPlay.text = "ИГРАТЬ";
                txtQuit.text = "ВЫХОД";
                txtTitle.text = $"Забрось мяч";
            }
            else
            {
                string s1 = $"Number of available attempts : {PlayerInfo.Instance.countLive}";
                string s2 = $"The number of points scored : {PlayerInfo.Instance.totalScore}";
                string s3 = $"Continue from level {PlayerInfo.Instance.currentLevel}";
                txtInfo.text = $"{s1}\n{s2}\n{s3}";
                txtPlay.text = "PLAY";
                txtQuit.text = "EXIT";
                txtTitle.text = $"Throw the ball\ninto the target";
            }
        }*/
    }
}

