using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to derive from for UI options to get input 
/// </summary>
public class OptionInputReceiver : MonoBehaviour
{
    /// <summary>
    /// Called whenever the user presses enter or X on controllers
    /// </summary>
    public virtual void OnReceiveXInput(){ }

    /// <summary>
    /// Called whenever the user gives horizontal input, by keyboard or controllers
    /// </summary>
    /// <param name="input">The horizontal input </param>
    public virtual void OnReceiveHorizontalInput(float input){}

    /// <summary>
    /// Called whenever the user gives vertical input, by keyboard or controllers
    /// </summary>
    /// <param name="input">The vertical input</param>
    public virtual void OnReceiveVerticalInput(float input) { }
}
