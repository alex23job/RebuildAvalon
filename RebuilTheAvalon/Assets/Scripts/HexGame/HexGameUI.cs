using UnityEngine;
using UnityEngine.UI;

public class HexGameUI : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text winText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ViewEndPanel(int zn)
    {
        if (Language.Instance.CurrentLanguage == "ru")
        {
            if (zn == 1)
            {
                winText.text = "Вы победили !";
            }
            else
            {
                winText.color = Color.red;
                winText.text = "Увы и ах! Ваш противник был первым !";
            }
        }
        else
        {
            if (zn == 1)
            {
                winText.text = "You've won !";
            }
            else
            {
                winText.color = Color.red;
                winText.text = "Alas and ah! Your opponent was the first!";
            }
        }
        endPanel.SetActive(true);
    }
}
