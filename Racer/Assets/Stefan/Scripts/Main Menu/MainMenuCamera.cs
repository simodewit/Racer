using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header ("Main Settings")]
    public float moveSmoothTime = 0.08f;
    public float rotationSmoothTime = 0.12f;

    [Header ("Start Screen")]
    public Transform startScreenCamPos;
    public bool useStartRotation;

    [Header ("Main Screen")]
    public Transform mainScreenCamPos;
    public Transform mainScreenCamPivot;
    public bool useMainRotation;

    [Header ("Car Selection Screen")]
    public Transform carScreenCamPos;
    public Transform carScreenCamPivot;
    public bool useCarRotation;

    [Header ("Map Selection Screen")]
    public Transform mapScreenCamPos;
    public Transform mapScreenCamPivot;
    public bool useMapRotation;

    public MainMenuManager.MenuState menuState;

    Vector3 moveVelocity, eulerVelocity;

    [ProButton]
    public void SetMenuState ( MainMenuManager.MenuState state )
    {
        menuState = state;
        Debug.Log (state);
    }

    private void Update ( )
    {
        Transform target = GetCurrentCamPosition ( );
        Transform pivot = GetCurrentCamPivot ( );

        bool useRotation = UseRotation ( );

        if ( target != null )
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref moveVelocity, moveSmoothTime);

            if ( useRotation )
            {
                transform.localEulerAngles = Vector3.SmoothDamp (transform.localEulerAngles, target.localEulerAngles, ref eulerVelocity, rotationSmoothTime);
            }
        }

        transform.SetParent (pivot);

    }

    private Transform GetCurrentCamPosition ( )
    {
        return menuState switch
        {
            MainMenuManager.MenuState.StartScreen => startScreenCamPos,
            MainMenuManager.MenuState.MainMenu => mainScreenCamPos,
            MainMenuManager.MenuState.CarSelection => carScreenCamPos,
            MainMenuManager.MenuState.MapSelection => mapScreenCamPos,
            _ => null,
        };
    }
    private Transform GetCurrentCamPivot ( )
    {
        return menuState switch
        {
            MainMenuManager.MenuState.MainMenu => mainScreenCamPivot,
            MainMenuManager.MenuState.CarSelection => carScreenCamPivot,
            MainMenuManager.MenuState.MapSelection => mapScreenCamPivot,
            _ => null,
        };
    }

    private bool UseRotation ( )
    {
        return menuState switch
        {
            MainMenuManager.MenuState.StartScreen => useStartRotation,
            MainMenuManager.MenuState.MainMenu => useMainRotation,
            MainMenuManager.MenuState.CarSelection => useCarRotation,
            MainMenuManager.MenuState.MapSelection => useMapRotation,
            _ => false,
        };
    }
}
