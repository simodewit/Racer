using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    private static PauseManager m_instance;

    /// <summary>
    /// The global static reference to the pause manager
    /// </summary>
    public static PauseManager Instance
    {
        get
        {
            if(m_instance == null )
            {
                m_instance = GameObject.FindObjectOfType<PauseManager> ( );
            }
            return m_instance;
        }
    }

    private bool m_paused;

    public UnityEvent onGamePaused,onGameContinue;

    /// <summary>
    /// The paused state of the game
    /// </summary>
    public bool Paused
    {
        get
        {
            return m_paused;
        }
        private set
        {
            if ( value == m_paused )
                return;

            m_paused = value;

            OnPauseStateChanged ( );
        }
    }

    private void OnPauseStateChanged ( )
    {
        if ( Paused )
        {
            onGamePaused.Invoke ( );
            Time.timeScale = 0;
        }
        else
        {
            onGameContinue.Invoke ( );
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Will toggle the pause state of the game
    /// </summary>
    /// <param name="paused"></param>
    [ProButton]
    public void SetPauseState (bool paused)
    {
        Paused = paused;
    }

    /// <summary>
    /// Will pause the game if it is not paused, and pause the game if its paused
    /// </summary>
    public void Toggle ( )
    {
        Paused = !Paused;
    }
}
