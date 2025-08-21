using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LettersGame : MonoBehaviour
{
    [SerializeField] private Button[] btnLetters;
    [SerializeField] private Button[] btnResultLetters;
    [SerializeField] private Text txtCount;

    private string word = "";
    private string resultWord = "";
    private int countCheck = 7;
    private Color[] colors = new Color[4] { new Color(0.5f, 1, 0.5f), Color.yellow, new Color(1f, 0.5f, 0.5f), new Color(0.8f, 0.8f, 0.8f) };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClearLetters(btnResultLetters);
        ClearLetters(btnLetters);
        SetGameParams("proba", 7, "probafreind");
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
        for (int i = 0; i < btnResultLetters.Length; i++)
        {
            if (i < len)
            {
                btnResultLetters[i].gameObject.SetActive(true);
                Vector3 pos = btnResultLetters[i].transform.localPosition;
                pos.x = startPos + i * 70;
                //print($"i={i}   btnPos={btnResultLetters[i].transform.position}   pos={pos}   localPos={btnResultLetters[i].transform.localPosition}");
                btnResultLetters[i].transform.localPosition = pos;
            }
            else
            {
                btnResultLetters[i].gameObject.SetActive(false);
            }
        }
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
        
    }

    public void OnResultClick(int num)
    {
        print($"Btn Result {num}");
    }

    public void OnCheckButtonClick()
    {
    }
}
