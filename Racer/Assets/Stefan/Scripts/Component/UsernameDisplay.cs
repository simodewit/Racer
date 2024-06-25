using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;

    private void Update ( )
    {
        UpdateText ( );
    }
    void UpdateText ( )
    {
        if ( textField == null )
            return;

        string userName = Authenticator.UserName;

        if(userName == null || userName.Length <= 0 )
            userName = "User";

        textField.text = userName;

    }
}
