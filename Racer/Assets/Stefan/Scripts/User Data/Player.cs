using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Player
{
    const string SAVE_FILE_NAME = "PlayerData.json";

    private static UserData _savedData;

    /// <summary>
    /// The UserData Saved on the hard drive
    /// </summary>
    public static UserData SavedData
    {
        get
        {
            if(_savedData == null )
            {
                string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

                if ( File.Exists (path) )
                {
                    string json = File.ReadAllText (path);

                    _savedData = JsonUtility.FromJson<UserData> (json) ?? UserData.DefaultData;
                }
                else
                {
                    _savedData = UserData.DefaultData;
                }
            }

            return _savedData;
        }
    }

    public static void SaveData(UserData data )
    {
        string json = JsonUtility.ToJson (data, true);

        string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

        File.WriteAllText(path, json);

        Debug.Log ("Saved Player Data");
    }

}
