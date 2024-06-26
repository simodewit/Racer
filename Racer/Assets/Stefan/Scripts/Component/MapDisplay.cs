using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [Header ("REferences")]
    public TextMeshProUGUI mapSizeText;
    public TextMeshProUGUI carText;

    private void Update ( )
    {
        var data = Player.SavedData;

        mapSizeText.text = data.SelectedMapSize;
        carText.text = data.SelectedCarName;
    }
}
