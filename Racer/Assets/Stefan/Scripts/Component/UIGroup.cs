using UnityEngine;

[RequireComponent (typeof (CanvasGroup))]
public class UIGroup : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [Header ("Settings")]
    public bool isActive = true;
    public float startDelay = 0;
    public float transitionTime = 0.2f;
    public bool delayOnStart = true;

    private float _transitionTimer;

    private void Awake ( )
    {
        canvasGroup = GetComponent<CanvasGroup> ( );
    }

    void Start ( )
    {
        _transitionTimer = transitionTime + startDelay;
    }

    private void Update ( )
    {
        float startTime = isActive && delayOnStart ? startDelay : 0;

        float endTime = isActive && delayOnStart ? startDelay + transitionTime : transitionTime;

        float progress = Mathf.InverseLerp (startTime, endTime, _transitionTimer);

        canvasGroup.alpha = Mathf.Lerp (isActive ? 0 : 1, isActive ? 1 : 0, progress);

        canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
        _transitionTimer += Time.deltaTime;

    }
    public void Toggle ( ) => Toggle (!isActive);
    public void Toggle ( bool toggled )
    {
        if ( toggled == isActive )
            return;

        _transitionTimer = 0;
        isActive = toggled;
    }

}
