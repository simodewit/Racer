using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelector : MonoBehaviour
{
    public ExampleCarData[] exampleCars;

    private int selectedCarIndex;

    [Header ("Settings")]
    public int startPadding;
    public int buttonWidth;
    public float smoothTime;


    [Header ("References")]
    public GameObject carButtonPrefab;
    public Transform carButtonsParent;
    public HorizontalLayoutGroup layout;

    float smoothVelocity;

    private void Start ( )
    {
        Initialize ( );
    }

    void Initialize ( )
    {
        foreach ( var car in exampleCars )
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
                if ( selectedCarIndex > 0 )
                {
                    SetButtonSelectedState (selectedCarIndex, false);
                    selectedCarIndex--;
                    SetButtonSelectedState (selectedCarIndex, true);
                }

            }
            else // Go Right
            {
                if ( selectedCarIndex < exampleCars.Length - 1 )
                {
                    SetButtonSelectedState (selectedCarIndex, false);
                    selectedCarIndex++;
                    SetButtonSelectedState (selectedCarIndex, true);
                }
            }
        }
    }

    void UpdateLayout ( )
    {
        int currentPadding = layout.padding.left;

        int targetPadding = startPadding - (buttonWidth + Mathf.RoundToInt( layout.spacing)) * selectedCarIndex;

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
