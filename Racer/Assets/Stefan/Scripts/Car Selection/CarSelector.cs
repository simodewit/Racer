using System.Collections;
using System.Collections.Generic;
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
        CheckInput ( );

        UpdateLayout ( );
    }

    void CheckInput ( )
    {
        if ( Input.GetButtonDown ("Horizontal") )
        {
            float hor = Input.GetAxisRaw ("Horizontal");

            if(hor < 0) // Go Left
            {
                if ( scrollSelectedCarIndex > 0 )
                {
                    SetButtonSelectedState (scrollSelectedCarIndex, false);
                    scrollSelectedCarIndex--;
                    SetButtonSelectedState (scrollSelectedCarIndex, true);

                    onScroll.Invoke ( );

                }

            }
            else // Go Right
            {
                if ( scrollSelectedCarIndex < cars.Length - 1 )
                {
                    SetButtonSelectedState (scrollSelectedCarIndex, false);
                    scrollSelectedCarIndex++;
                    SetButtonSelectedState (scrollSelectedCarIndex, true);

                    onScroll.Invoke ( );
                }
            }
        }
    }

    public void TrySelect (InputAction.CallbackContext context)
    {
        if ( scrollSelectedCarIndex == selectedCarIndex )
            return;

        selectedCarIndex = scrollSelectedCarIndex;

        Debug.Log ($"Selected index {selectedCarIndex}");

        onCarSelected.Invoke ( );
    }

    void UpdateLayout ( )
    {
        int currentPadding = layout.padding.left;

        int targetPadding = startPadding - (buttonWidth + Mathf.RoundToInt( layout.spacing)) * scrollSelectedCarIndex;

        layout.padding.left = Mathf.RoundToInt( Mathf.SmoothDamp (currentPadding, targetPadding, ref smoothVelocity, smoothTime));
        layout.SetLayoutHorizontal ( );
    }

    void SetButtonSelectedState(int index, bool selected )
    {
        if(index > carButtonsParent.childCount - 1 )
        {
            Debug.Log ($"Index {index} larger than childcount {carButtonsParent.childCount}");
            return;
        }

        Transform child =  carButtonsParent.GetChild (index);

        if ( child )
        {
            child.GetComponent<CarButton> ( ).SetSelected (selected);
        }
    }
}
