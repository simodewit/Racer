using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    public MapData[] maps;

    [Header ("Settings")]
    public float mapMovementSpeed;
    public float mapZoomSpeed;
    public Vector2 mapBounds;
    public Vector2 zoomBounds;
    public float startZoom;
    public Vector2 startPos;

    [Header ("References")]
    public OptionInputReceiver sizeSelector;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI prText;
    public Image mapImage;
    public Transform scalePivot;

    [Header ("Input")]
    public PlayerInput input;

    //Private Variables

    private float _zoom;
    private float _zoomInput;
    private bool _isZooming;

    private void Update ( )
    {
        HandleZoom ( );
    }
    #region Size Selection
    //Size Selection

    public void PreviousSize(InputAction.CallbackContext context )
    {
        if(context.phase == InputActionPhase.Started)
            sizeSelector.OnReceiveHorizontalInput (-1);
    }

    public void NextSize(InputAction.CallbackContext context )
    {
        if ( context.phase == InputActionPhase.Started )
            sizeSelector.OnReceiveHorizontalInput (1);
    }
    #endregion

    #region Map 

    public void MapMovement ( InputAction.CallbackContext context )
    {
        Vector2 movement = mapMovementSpeed * Time.deltaTime * context.ReadValue<Vector2> ( );

        Vector2 targetPos = (Vector2)mapImage.transform.localPosition + movement;

        targetPos.x = Mathf.Clamp (targetPos.x, -mapBounds.x, mapBounds.x);
        targetPos.y = Mathf.Clamp (targetPos.y, -mapBounds.y, mapBounds.y);

        mapImage.transform.localPosition = targetPos;
    }

    public void MapZooming ( InputAction.CallbackContext context )
    {
        if ( context.phase == InputActionPhase.Started )
            _isZooming = true;
        else if ( context.phase == InputActionPhase.Canceled )
            _isZooming = false;

        _zoomInput = context.ReadValue<float> ( );
    }

    private void HandleZoom ( )
    {
        if ( _isZooming )
        {
            _zoom += _zoomInput * mapZoomSpeed * Time.deltaTime;

            _zoom = Mathf.Clamp (_zoom, zoomBounds.x, zoomBounds.y);

            scalePivot.localScale = _zoom * Vector3.one;
        }
    }

    public void OnMapIndexChanged(int index )
    {
        MapData currentMap = maps[index];

        distanceText.text = $"{currentMap.distance:F2} KM";

        prText.text = TimeSpan.FromSeconds (currentMap.prTime).ToString ();

        mapImage.sprite = currentMap.mapSprite;
    }

    public void ResetMap ( )
    {
        scalePivot.localScale = Vector3.one * startZoom;
        _zoom = startZoom;

        mapImage.transform.localPosition = startPos;
    }

    #endregion

    #region Other Input
    public void Back (InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.SetMenuState (MainMenuManager.MenuState.MainMenu);
    }

    public void Options(InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.ToOptionsMenu ( );
    }
    #endregion

    public void OnScreenEnable ( )
    {
        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        input.enabled = false;
    }
}
