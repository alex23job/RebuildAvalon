using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Control : MonoBehaviour
{
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private Text hintText;

    private bool isHint = false;
    private float timerHint = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHint)
        {
            if (timerHint > 0) timerHint -= Time.deltaTime;
            else
            {
                isHint = false;
                hintPanel.SetActive(false);
            }
        }
    }

    public void LoadMenuScene()
    {
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("MainScene");
    }

    public void ViewHint(string txtHint)
    {
        hintText.text = txtHint;
        hintPanel.SetActive(true);
        timerHint = 5f;
        isHint = true;
    }
}
