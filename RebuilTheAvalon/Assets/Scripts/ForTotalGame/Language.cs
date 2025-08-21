using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Language : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetLang();

    public string CurrentLanguage = "ru";  //  ru en
    //public string CurrentLanguage = "en";  //  ru en

    public static Language Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Invoke("LoadLanguage", 0.02f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadLanguage()
    {
#if UNITY_WEBGL
        CurrentLanguage = GetLang();
#endif
        //CurrentLanguage = "ru";
        //CurrentLanguage = "en";
    }
}

