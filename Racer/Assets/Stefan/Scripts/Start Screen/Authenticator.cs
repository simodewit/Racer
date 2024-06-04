using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Authenticator
{
    const string SAVE_FILE_NAME = "AuthData.json";

    private static AuthData _data;

    private static AuthData AuthenticatorData
    {
        get
        {
            if(_data == null )
            {
                string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

                if ( File.Exists (path) )
                {
                    string json = File.ReadAllText (path);

                    _data = JsonUtility.FromJson<AuthData> (json);
                }
                else
                {
                    _data = new AuthData
                    {
                        name = null,
                        hasSignedIn = false,
                    };
                }
            }
            return _data;
        }
    }

    /// <summary>
    /// Has the user created an name?
    /// </summary>
    public static bool IsSignedIn
    {
        get
        {
            return AuthenticatorData.hasSignedIn;
        }
    }

    /// <summary>
    /// The username of the player, returns null if the user hasn't picked a name yet
    /// </summary>
    public static string UserName
    {
        get
        {
            return AuthenticatorData.name;
        }
    }

    /// <summary>
    /// Creates a user and saves the name
    /// </summary>
    /// <param name="name"></param>
    public static void CreateUser(string name )
    {
        AuthenticatorData.name = name;
        AuthenticatorData.hasSignedIn = true;
    }

    public static void Save ( )
    {
        string json = JsonUtility.ToJson ( AuthenticatorData );

        string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

        File.WriteAllText (path, json);

        Debug.Log ("Saved!");

    }

    private class AuthData
    {
        public string name;

        public bool hasSignedIn;
    }
}
