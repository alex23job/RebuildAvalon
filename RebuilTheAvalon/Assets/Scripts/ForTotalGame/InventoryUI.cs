using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private Button btnPrev;
    [SerializeField] private Button btnNext;
    [SerializeField] private GameObject selectPanel;
    [SerializeField] private Button btnUse;

    private int currentItem = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ViewInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPrevClick()
    {
        if (currentItem > 0)
        {
            currentItem--;
            ViewInventory();
        }
    }

    public void OnNextClick()
    {
        if (currentItem < GameManager.Instance.currentPlayer.inventory.CountItem - 4)
        {
            currentItem++;
            ViewInventory();
        }
    }

    public void OnItemClick(int num)
    {
        InventoryItem item = GameManager.Instance.currentPlayer.inventory.GetItem(currentItem + num);
        if (item != null)
        {
            selectPanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = item.ItemName;
            selectPanel.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = item.Sprite;
            selectPanel.SetActive(true);
        }
    }

    public void ViewInventory()
    {
        int i, countItems = GameManager.Instance.currentPlayer.inventory.CountItem;
        for (i = 0; i < 4; i++)
        {
            if (currentItem + i < countItems)
            {
                InventoryItem item = GameManager.Instance.currentPlayer.inventory.GetItem(currentItem + i);
                images[i].sprite = item.Sprite;
            }
            else
            {
                images[i].sprite = null;
            }
        }
        btnPrev.interactable = (currentItem != 0);
        btnNext.interactable = (currentItem < GameManager.Instance.currentPlayer.inventory.CountItem - 4);
    }

    public void AssignMethod(Action<string> method, string parameter)
    {
        // Добавляем метод в событие OnClick кнопки с передачей параметра
        btnUse.onClick.AddListener(() => method(parameter));
    }
}
