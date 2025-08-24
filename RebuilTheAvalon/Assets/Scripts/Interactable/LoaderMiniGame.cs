using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderMiniGame : MonoBehaviour, IInteractable
{
    [SerializeField] private string nameScene;
    [SerializeField] private string IDS_Game;
    [SerializeField] private PlayerInteract player;
    public void EndInteract()
    {
        throw new System.NotImplementedException();
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
        SceneManager.LoadScene(nameScene);
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
