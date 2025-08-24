using UnityEngine;

public class DoorControl : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = -52.0f;
    private Animator anim;
    private float endAngle;
    private bool isOpen = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void DoorOpen()
    {
        //anim.SetTrigger("OpenTrigger");
        endAngle = openAngle;
        Invoke("SetAngleY", 1.05f);
        isOpen = true;
        anim.SetBool("IsOpen", isOpen);
    }

    public void DoorClose()
    {
        //anim.SetTrigger("CloseTrigger");
        endAngle = 0;
        Invoke("SetAngleY", 1.05f);
        isOpen = false;
        anim.SetBool("IsOpen", isOpen);
    }

    private void SetAngleY()
    {
        transform.rotation = Quaternion.Euler(0, endAngle, 0);
    }

    public void Interact()
    {
        DoorOpen();
    }

    public void EndInteract()
    {
        DoorClose();
    }

    public string GetHint()
    {
        string actionStr = (!isOpen) ? "open" : "close";
        string hintStr = $"Press \'E\' to {actionStr} the door";
        if (Language.Instance.CurrentLanguage == "ru")
        {
            actionStr = (!isOpen) ? "открыть" : "закрыть";
            hintStr = $"Нажмите \'Е\' чтобы {actionStr} дверь";
        }       
        return hintStr;
    }
}
