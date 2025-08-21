using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory
{
    private List<InventoryItem> items = new List<InventoryItem>();

    public int CountItem { get => items.Count; }

    public Inventory() { }

    public Inventory(string csv, char sep = '#')
    {
        if (csv == null || csv == "") return;
        string[] ar = csv.Split(sep);
        items.Clear();
        foreach(string s in ar)
        {
            items.Add(new InventoryItem(s, '='));
        }
    }
    public void AddItem(InventoryItem item)
    {
        InventoryItem testItem = GetItem(item.ItemName);
        if (testItem == null)
        {
            items.Add(new InventoryItem(item.ItemID, item.ItemName, item.Sprite));
            Debug.Log($"inventory.AddItem item={item.ToString()}  count={items.Count}");
        }
    }

    public InventoryItem GetItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    public InventoryItem GetItem(string name)
    {
        foreach(InventoryItem item in items)
        {
            if (item.ItemName == name) return item;
        }
        return null;
    }
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
        }
    }

    public string ToCsvString(char sep = '#')
    {
        StringBuilder sb = new StringBuilder();
        foreach (InventoryItem item in items)
        {
            sb.Append($"{item.ToCsvString()}{sep}"); 
        }
        return sb.ToString();
    }

    public void SetSprites(string nm, Sprite spr)
    {
        foreach(InventoryItem item in items)
        {
            if (item.ItemName == nm)
            {
                item.SetSprite(spr);
                break;
            }
        }
    }
}

[Serializable]
public class InventoryItem
{
    private int itemID;
    private string itemName;
    private Sprite sprite;

    public InventoryItem() { }
    public InventoryItem(int id, string nm, Sprite spr)
    {
        ItemID = id;
        ItemName = nm;
        Sprite = spr;
    }

    public InventoryItem(string csv, char sep = '=')
    {
        string[] ar = csv.Split(sep); 
        if (ar.Length >= 2)
        {
            if (int.TryParse(ar[0], out int id))
            {
                ItemID = id;
                ItemName = ar[1];
                Sprite = null;
            }
        }
    }

    public int ItemID { get => itemID; private set => itemID = value; }
    public string ItemName { get => itemName; private set => itemName = value; }
    public Sprite Sprite { get => sprite; private set => sprite = value; }

    public void SetSprite(Sprite spr)
    {
        sprite = spr;
    }

    public string ToCsvString(char sep='=')
    {
        return $"{itemID}{sep}{itemName}{sep}";
    }

    public override string ToString()
    {
        return $"id={ItemID} name={ItemName}";
    }
}
