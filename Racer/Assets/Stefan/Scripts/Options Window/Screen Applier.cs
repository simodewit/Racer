using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenApplier : MonoBehaviour
{
    public Res[] resolutions;
    [System.Serializable]
    public struct Res
    {
        public int width;
        public int height;

    }
    public void ApplyScreen ( )
    {
        var data = OptionsData.Saved;

        Screen.SetResolution (resolutions[data.resolutionIndex].width, resolutions[data.resolutionIndex].height, data.fullscreenMode);
    }
}
