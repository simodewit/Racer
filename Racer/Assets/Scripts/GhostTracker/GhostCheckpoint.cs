using UnityEngine;

public class GhostCheckpoint : MonoBehaviour
{
    #region variables

    [Tooltip("The ghost replay script for the best lap")]
    [SerializeField] private GhostPlayer bestLapData;
    [Tooltip("The ghost replay script for the last lap")]
    [SerializeField] private GhostPlayer lastLapData;
    [Tooltip("The ScriptableObject that is going to store the fastests driven data")]
    [SerializeField] private GhostData bestData;
    [Tooltip("The ScriptableObject that is going to store the current driven data")]
    [SerializeField] private GhostData lastData;
    [Tooltip("The amount of times that the player is tracked per second")]
    [SerializeField] private float recordFrequency;

    private GhostData prevData;

    #endregion

    #region collision

    public void OnTriggerEnter(Collider other)
    {
        ManageRecordings();
    }

    #endregion

    #region manager

    private void ManageRecordings()
    {
        if(lastData != null)
        {

        }
    }

    #endregion
}
