using UnityEngine;

public class ItemForInventory : MonoBehaviour, IInteractable
{
    [SerializeField] private string ids;

    public void EndInteract()
    {
        Debug.Log("ItemForInventory EndInteract");
    }

    public string GetHint()
    {
        return "Нажмите \'Е\' чтобы взять предмет";
    }

    public void Interact()
    {
        Debug.Log("ItemForInventory Interact");
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
