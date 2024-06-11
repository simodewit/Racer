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

    [Header ("Settings")]
    public Vector3 hoveredScale;
    public Vector3 unhoveredScale;
    public float hoverSmoothTime;

    private Vector3 _velocity;
    private bool _hovered;
    private bool _selected;
    private CarObject _car;

    private void Awake ( )
    {
        animator = GetComponent<Animator> ( );
    }

    private void Update ( )
    {
        Vector3 target = _hovered ? hoveredScale : unhoveredScale;
        carImg.transform.localScale = Vector3.SmoothDamp (carImg.transform.localScale, target, ref _velocity, hoverSmoothTime);
    }

    public void Initialize ( CarObject carData )
    {
        carImg.sprite = carData.carImage;
        brandImg.sprite = carData.brandSprite;
        carColorImg.color = carData.carColor;

        carNameText.text = carData.FullName;

        selectText.text = $"Select {carData.FullName}!";

        lockedObject.SetActive (!carData.unlocked);
        animator.SetBool ("Unlocked", carData.unlocked);

        _car = carData;
    }

    public void SetHovered ( bool hovererd )
    {
        _hovered = hovererd;
        animator.SetBool ("Hovered", hovererd);
    }

    public void SetSelected ( bool selected )
    {
        _selected = selected;
        animator.SetBool ("Selected", selected);

        if ( selected )
        {
            selectText.text = $"Already Selected {_car.FullName}";
        }
        else
        {
            selectText.text = $"Select {_car.FullName}!";
        }
    }
}
