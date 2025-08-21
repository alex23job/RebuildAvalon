using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class NoteSet : MonoBehaviour
{
    private List<NoteItem> notes = new List<NoteItem>();

    public static NoteSet Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Invoke("LoadNotes", 0.001f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadNotes()
    {
        if (File.Exists("NoteData.txt"))
        {
            string csv = File.ReadAllText("NoteData.txt", System.Text.Encoding.UTF8);
            CreateNotes(csv);
            //BinaryFormatter bf = new BinaryFormatter();
            ////Debug.Log(Application.persistentDataPath + "/MySaveData.dat");
            //FileStream file = File.Open("NoteData.txt", FileMode.Open);
            //MyNoteData data = (MyNoteData)bf.Deserialize(file);
            //file.Close();
            //Debug.Log(data.ToString());
            //CreateNotes(data.notesCsv);
            
            Debug.Log($"Загружено {notes.Count} текстов записок");
        }
        else
        {
            Debug.Log("There is no save data! File NoteData.txt not found.");
        }
    }

    private void CreateNotes(string csv, char sep = '#')
    {
        notes.Clear();
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        foreach(string item in ar)
        {
            if (item == "") continue;
            NoteItem note = new NoteItem(item, '=');
            if (note.IDS_NOTE != "") notes.Add(note);
        }
    }

    public string GetNote(string ids, string lang)
    {
        foreach(NoteItem item in notes)
        {
            if (item.IDS_NOTE == ids)
            {
                string note = item.GetNote(lang);
                if (note != "") return note;
                else break;
            }
        }
        return ids;
    }
}

[Serializable]
public class MyNoteData
{
    public string notesCsv = "";
}
