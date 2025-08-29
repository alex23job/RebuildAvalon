using UnityEngine;
using UnityEngine.UI;

public class MittenGameUI : MonoBehaviour
{
    [SerializeField] private Text txtPlayer;
    [SerializeField] private Text txtEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ViewResult(int value, int npc)
    {
        string resPlName = "Вы : ";
        string resEnName = "Агата : ";
        if (Language.Instance.CurrentLanguage != "ru")
        {
            resPlName = "You : ";
            resEnName = "Agatha : ";
        }
        if (npc == 1)
        {
            txtPlayer.text = $"{resPlName}{value}";
        }
        if (npc == 2)
        {
            txtEnemy.text = $"{resEnName}{value}";
        }
    }
}
