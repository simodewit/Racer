using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public CarObject[] cars;

    private int scrollSelectedCarIndex;
    private int selectedCarIndex;

    [Header ("Settings")]
    public int startPadding;
    public int buttonWidth;
    public float smoothTime;


    [Header ("References")]
    public GameObject carButtonPrefab;
    public Transform carButtonsParent;
    public HorizontalLayoutGroup layout;
    public CarPreview preview;
    public PlayerInput input;

    [Header ("Events")]
    public UnityEvent onScroll;
    public UnityEvent onCarSelected;

    float smoothVelocity;

    public CarObject ScrollSelectedCar
    {
        get
        {
            return cars[scrollSelectedCarIndex];
        }
    }

    public CarObject SelectedCar
    {
        get
        {
            return cars[selectedCarIndex];
        }
    }

    private void Start ( )
    {
        Initialize ( );
    }

    void Initialize ( )
    {
        foreach ( var car in cars )
        {
            CarButton button = Instantiate (carButtonPrefab, carButtonsParent).GetComponent<CarButton> ( );

            button.Initialize (car);
        }
    }

    private void Update ( )
    {
        UpdateLayout ( );
    }

    #region Input
    public void ScrollInput ( InputAction.CallbackContext context )
    {
        if ( context.phase == InputActionPhase.Started )
        {
            float hor = context.ReadValue<float> ( );

            if ( hor < 0 ) // Go Left
            {
                if ( scrollSelectedCarIndex > 0 )
                {
                    Scroll (scrollSelectedCarIndex - 1);

                }

            }
            else // Go Right
            {
                if ( scrollSelectedCarIndex < cars.Length - 1 )
                {
                    Scroll (scrollSelectedCarIndex + 1);
                }
            }
        }
    }

    private void Scroll(int newIndex )
    {
        SetButtonSelectedState (scrollSelectedCarIndex, false);
        scrollSelectedCarIndex = newIndex;
        SetButtonSelectedState (scrollSelectedCarIndex, true);

        onScroll.Invoke ( );

        SetPreviewCar (scrollSelectedCarIndex);
    }


    public void TrySelect ( InputAction.CallbackContext context )
    {
        if ( scrollSelectedCarIndex == selectedCarIndex )
            return;

        Transform toDeselect = carButtonsParent.GetChild (selectedCarIndex);

        if ( toDeselect )
        {
            toDeselect.GetComponent<CarButton> ( ).SetSelected (false);
        }

        selectedCarIndex = scrollSelectedCarIndex;

        Transform toSelect = carButtonsParent.GetChild (selectedCarIndex);

        if ( toSelect )
        {
            toSelect.GetComponent<CarButton> ( ).SetSelected (true);
        }

        Debug.Log ($"Selected index {selectedCarIndex}");

        onCarSelected.Invoke ( );
    }

    public void ToMenu(InputAction.CallbackContext context )
    {
        MainMenuManager.Instance.ToOptionsMenu ( );
    }

    #endregion

    public void SetPreviewCar ( int index )
    {
        preview.SetPrefab (cars[index].showcasePrefab);
    }

    void UpdateLayout ( )
    {
        int currentPadding = layout.padding.left;

        int targetPadding = startPadding - ( buttonWidth + Mathf.RoundToInt (layout.spacing) ) * scrollSelectedCarIndex;

        layout.padding.left = Mathf.RoundToInt (Mathf.SmoothDamp (currentPadding, targetPadding, ref smoothVelocity, smoothTime));
        layout.SetLayoutHorizontal ( );
    }

    void SetButtonSelectedState ( int index, bool selected )
    {
        if ( index > carButtonsParent.childCount - 1 )
        {
            Debug.Log ($"Index {index} larger than childcount {carButtonsParent.childCount}");
            return;
        }

        Transform child = carButtonsParent.GetChild (index);

        if ( child )
        {
            child.GetComponent<CarButton> ( ).SetHovered (selected);
        }
    }

    #region Active Events
    public void OnScreenEnable ( )
    {
        //Select selected car
        //set scroll index to selected car

        //TODO: Read selected and saved car index
        int savedSelectedCarIndex = 0;

        selectedCarIndex = savedSelectedCarIndex;
        scrollSelectedCarIndex = savedSelectedCarIndex;

        Scroll (scrollSelectedCarIndex);

        SetPreviewCar (selectedCarIndex);

        input.enabled = true;
    }

    public void OnScreenDisable ( )
    {
        input.enabled = false;
    }

    #endregion
}
