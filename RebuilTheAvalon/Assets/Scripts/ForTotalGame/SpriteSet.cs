using System;
using UnityEngine;

[Serializable]
public class SpriteSet : MonoBehaviour
{
    public static SpriteSet Instance;

    [SerializeField] private MySprite[] mySprites;

    public int CountMySprites { get => mySprites.Length; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetSprite(string nm)
    {
        foreach(MySprite ms in mySprites)
        {
            if (ms.name == nm) return ms.sprite;
        }
        return null;
    }
}

[Serializable]
public class MySprite
{
    public string name;
    public Sprite sprite;
}