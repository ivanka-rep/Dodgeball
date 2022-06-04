using UnityEngine;
using UnityEditor;
  
public class DeletePlayerPrefs : EditorWindow
{
    [MenuItem("PlayerPrefs/Clear")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}