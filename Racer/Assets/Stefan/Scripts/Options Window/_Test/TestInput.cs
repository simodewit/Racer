using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : OptionInputReceiver
{
    public UIOption option;

    public override void OnReceiveHorizontalInput ( System.Single input )
    {
        if ( input == 0 )
            return;

        Debug.Log ($"Received {input} horizontal input on option: {option.optionName}");
    }
    public override void OnReceiveVerticalInput ( System.Single input )
    {
        if(input == 0)
            return;

        Debug.Log ($"Received {input} vertical input on option: {option.optionName}");
    }
    public override void OnReceiveXInput ()
    {
        Debug.Log ($"Received X input on option: {option.optionName}");
    }
}
