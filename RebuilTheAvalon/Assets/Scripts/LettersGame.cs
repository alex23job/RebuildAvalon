using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LettersGame : MonoBehaviour
{
    [SerializeField] private Button[] btnLetters;
    [SerializeField] private Button[] btnResultLetters;
    [SerializeField] private Text txtCount;
    [SerializeField] private Button btnCheck;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text txtEnd;

    private string word = "";
    private string resultWord = "";
    private int countCheck = 7;
    private Color[] colors = new Color[4] { new Color(0.5f, 1, 0.5f), Color.yellow, new Color(1f, 0.5f, 0.5f), new Color(0.8f, 0.8f, 0.8f) };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClearLetters(btnResultLetters);
        ClearLetters(btnLetters);
        //SetGameParams("proba", 7, "probafinds");
        SetGameParams("книга", 7, "книгаморды");
        btnCheck.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameParams(string resWord, int countCh, string letters)
    {
        resultWord = resWord;
        countCheck = countCh;
        List<int> nums = new List<int>();
        int i, zn;
        for (i = 0; i < btnLetters.Length; i++) nums.Add(i);
        for (i = 0; i < btnLetters.Length; i++)
        {
            zn = nums[Random.Range(0, nums.Count)];
            if (zn < letters.Length)
            {
                btnLetters[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = letters.Substring(zn, 1);
            }
            nums.Remove(zn);
        }
        ViewAttempts();
        ViewResultButtons();
        //for (i = 0; i < btnResultLetters.Length; i++) btnResultLetters[i].GetComponent<Image>().color = Color.yellow;
        //for (i = 0; i < btnResultLetters.Length; i++) SetBtnResultColor(1, btnResultLetters[i]);
        foreach (Button btn in btnResultLetters) SetBtnResultColor(3, btn);
    }

    private void ViewResultButtons()
    {
        int len = resultWord.Length;
        int startPos = -210 + 35 * (7 - len);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < btnResultLetters.Length; i++)
        {
            if (i < len)
            {
                btnResultLetters[i].gameObject.SetActive(true);
                Vector3 pos = btnResultLetters[i].transform.localPosition;
                pos.x = startPos + i * 70;
                //print($"i={i}   btnPos={btnResultLetters[i].transform.position}   pos={pos}   localPos={btnResultLetters[i].transform.localPosition}");
                btnResultLetters[i].transform.localPosition = pos;
                sb.Append("?");
            }
            else
            {
                btnResultLetters[i].gameObject.SetActive(false);
            }
        }
        word = sb.ToString();
    }

    private void ViewResultWord()
    {
        int len = resultWord.Length;

    }

    private void SetBtnResultColor(int numColor, Button btn)
    {
        Color col = colors[3];
        if (numColor >= 0 && numColor < colors.Length) col = colors[numColor];
        btn.GetComponent<Image>().color = col;
    }

    private void ViewAttempts()
    {
        string ss = (Language.Instance.CurrentLanguage == "ru") ? "Попытки" : "Attempts";
        txtCount.text = $"{ss} : {countCheck}";
    }

    private void ClearLetters(Button[] arrBtn)
    {
        for (int i = 0; i < arrBtn.Length; i++)
        {
            arrBtn[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = "?";
        }
    }

    public void OnLetterClick(int num)
    {
        bool isFill = false;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == '?' && isFill == false)
            {
                string s = btnLetters[num].transform.GetChild(0).gameObject.GetComponent<Text>().text;
                sb.Append(s);
                btnResultLetters[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = s;
                isFill = true;
            }
            else sb.Append(word[i]);
        }
        word = sb.ToString();
    }

    public void OnResultClick(int num)
    {
        print($"Btn Result {num}");
        if (word[num] != '?')
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < word.Length; i++) 
            {
                if (i != num) sb.Append(word[i]);
                else
                {
                    sb.Append("?");
                    btnResultLetters[num].transform.GetChild(0).gameObject.GetComponent<Text>().text = "?";
                }
            }
            word = sb.ToString();
        }
    }

    public void OnCheckButtonClick()
    {
        if (countCheck > 0)
        {
            countCheck--;
            ViewAttempts();
            if (countCheck <= 0)
            {   //  loss
                btnCheck.interactable = false;
                txtEnd.text = (Language.Instance.CurrentLanguage == "ru") ? "Слово не угадано" : "The word is not guessed";
                txtEnd.color = new Color(0.6f, 0, 0);
                endPanel.SetActive(true);
            }
        }
        else return;

        for (int i = 0; i < resultWord.Length; i++)
        {
            if (word[i] == resultWord[i]) SetBtnResultColor(0, btnResultLetters[i]);
            else
            {
                if (resultWord.Contains(word[i])) SetBtnResultColor(1, btnResultLetters[i]);
                else SetBtnResultColor(2, btnResultLetters[i]);
            }
        }

        if (word == resultWord)
        {   //  win 
            string s = (Language.Instance.CurrentLanguage == "ru") ? "Угадано слово" : "The word is guessed";
            txtEnd.text = $"{s}   \"{resultWord}\"";
            txtEnd.color = new Color(0, 0.8f, 0);
            endPanel.SetActive(true);
            GameManager.Instance.currentPlayer.totalScore += 10;
        }
    }
}
