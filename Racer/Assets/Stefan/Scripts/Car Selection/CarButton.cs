using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarButton : MonoBehaviour
{
    private Animator animator;

    [Header ("References")]
    public Image carImg;
    public Image brandImg;
    public Image carColorImg;

    public TextMeshProUGUI carNameText, selectText;
    public GameObject lockedObject;

    private void Awake ( )
    {
        animator = GetComponent<Animator> ( );
    }

    public void Initialize ( ExampleCarData carData )
    {
        carImg.sprite = carData.carImage;
        brandImg.sprite = carData.brandLogo;
        carColorImg.color = carData.carColor;

        carNameText.text = carData.FullName;

        selectText.text = $"Select {carData.FullName}!";

        lockedObject.SetActive (!carData.unlocked);
        animator.SetBool ("Unlocked", carData.unlocked);

    }

    public void SetSelected ( bool selected )
    {
        animator.SetBool ("Selected", selected);
    }
}
