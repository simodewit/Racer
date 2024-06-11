using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class that contains save data for option the user can change
/// </summary>
[System.Serializable]
public class OptionsData
{
    private const string SAVE_FILE_NAME = "Options.json";

    private static OptionsData m_saved;

    /// <summary>
    /// The OptionsData saved on the local hard drive
    /// </summary>
    public static OptionsData Saved
    {
        get
        {
            if(m_saved == null )
            {
                string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

                if ( File.Exists (path) )
                {
                    string json = File.ReadAllText (path);

                    m_saved = JsonUtility.FromJson<OptionsData>(json);
                }
                else
                {
                    m_saved = DefaultOptions;
                }
            }
            return m_saved;
        }
    }

    /* ==================================================================== */
    /* VVVVV Here are all the options that can be changed in the game VVVVV */

    public int resolutionIndex;
    public int frameRateIndex;
    public bool fullscreenMode;
    public float gamma;
    public bool VolumetricClouds;

    public float mainVolume;
    public float uiVolume;
    public float sfxVolume;
    public float musicVolume;


    /* ==================================================================== */
    /// <summary>
    /// The standard/default options for the game
    /// </summary>
    public static OptionsData DefaultOptions
    {
        get
        {
            return new OptionsData
            {
                resolutionIndex = 0,
                frameRateIndex = 0,
                fullscreenMode = true,
                gamma = 1,
                VolumetricClouds = true,
                mainVolume = 1,
                uiVolume = 1,
                sfxVolume = 1,
                musicVolume = 1,
            };
        }
    }

    /// <summary>
    /// Write this OptionsData to the hard drive
    /// </summary>
    public void Save ( )
    {
        string json = JsonUtility.ToJson (this,true);

        string path = Path.Combine (Application.persistentDataPath, SAVE_FILE_NAME);

        File.WriteAllText (path, json);

        Debug.Log ("Saved Options");
    }
}
