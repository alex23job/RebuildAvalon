using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderMiniGame : MonoBehaviour, IInteractable
{
    [SerializeField] private string nameScene;
    [SerializeField] private string IDS_Game;
    [SerializeField] private GameObject panel_UI_Game = null;
    [SerializeField] private PlayerInteract player;
    public void EndInteract()
    {
        if (panel_UI_Game != null) panel_UI_Game.SetActive(false);
    }

    public string GetHint()
    {
        string lang = Language.Instance.CurrentLanguage;
        string strGame = NoteSet.Instance.GetNote(IDS_Game, lang);
        string hintStr = $"Press \'E\' to play {strGame}";
        if (lang == "ru")
        {
            hintStr = $"Нажмите \'Е\' чтобы сыграть в {strGame}";
        }
        return hintStr;
    }

    public void Interact()
    {
        player.SavePosAndRot();
        if (panel_UI_Game != null)
        {   //  надо вызывать какой-то метод класса UI игры для инициализации переменных игры 
            panel_UI_Game.SetActive(true);
        }
        else SceneManager.LoadScene(nameScene);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
