using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    List<UIOption> allOptions;

    [Header ("Settings")]
    public Sprite emptyOptionSprite;

    [Header ("References")]
    public Image optionImage;
    public TextMeshProUGUI optionNameText,optionDescriptionText;

    [SerializeField]
    private int currentOptionIndex;

    private void Start ( )
    {
        OnCurrentOptionIndexChanged ( );
    }

    private void Update ( )
    {
        CheckScrolling ( );

        HandleInput ( );
    }

    void CheckScrolling ( )
    {
        if ( !Input.GetButtonDown ("Vertical"))
            return;

        if ( Input.GetAxisRaw ("Vertical") < 0 ) // scrolling down
        {
            if ( currentOptionIndex < allOptions.Count - 1 )
            {
                SetCurrentOptionIndex (currentOptionIndex + 1);
            }
        }
        else //Scrolling up
        {
            if ( currentOptionIndex > 0)
            {
                SetCurrentOptionIndex (currentOptionIndex - 1);
            }
        }
    }

    void HandleInput ( )
    {
        UIOption option = allOptions[currentOptionIndex];

        if ( option == null || option.inputReceiver == null )
            return;

        //TODO: Change to new input system
        float horizontalInput = Input.GetAxisRaw ("Horizontal");

        if(horizontalInput != 0 )
        {
            option.inputReceiver.OnReceiveHorizontalInput (horizontalInput);
        }

        //float verticalInput = Input.GetAxisRaw ("Vertical");

        //if(verticalInput != 0 )
        //{
        //    option.inputReceiver.OnReceiveVerticalInput (verticalInput);
        //}

        //TODO: Add controller input support
        if ( Input.GetButtonDown ("Jump"))
        {
            option.inputReceiver.OnReceiveXInput ( );
        }
    }

    private void SetCurrentOptionIndex(int newIndex )
    {
        allOptions[currentOptionIndex].SetSelected (false);
        currentOptionIndex = newIndex;
        allOptions[currentOptionIndex].SetSelected (true);

        OnCurrentOptionIndexChanged ( );
    }
    private void OnCurrentOptionIndexChanged ( )
    {
        UIOption currentOption = allOptions[currentOptionIndex];

        if ( currentOption == null )
            return;

        if(currentOption.optionSprite != null )
        {
            optionImage.sprite = currentOption.optionSprite;
        }
        else
        {
            optionImage.sprite = emptyOptionSprite;
        }

        optionNameText.text = currentOption.optionName;
        optionDescriptionText.text = currentOption.optionDescription;
    }

}

