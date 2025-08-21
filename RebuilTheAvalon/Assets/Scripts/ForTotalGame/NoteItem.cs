using UnityEngine;
using System;

[Serializable]
public class NoteItem
{
    private string ids_note;
    private string noteRu;
    private string noteEn;

    public NoteItem() { }

    public NoteItem(string ids, string ru, string en)
    {
        ids_note = ids;
        noteRu = ru;
        noteEn = en;
    }

    public NoteItem(string csv, char sep = '=')
    {
        ids_note = "";
        string[] ar = csv.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        if (ar.Length >= 3)
        {
            ids_note = ar[0];
            noteRu = ar[1];
            noteEn = ar[2];
        }
    }

    public string IDS_NOTE { get => ids_note; }

    public string GetNote(string lang)
    {
        if (lang == "ru") return noteRu;
        if (lang == "en") return noteEn;
        return ids_note;
    }

    public string ToCsvString(char sep = '=')
    {
        return $"{ids_note}{sep}{noteRu}{sep}{noteEn}{sep}";
    }
}
