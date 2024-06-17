using System.Reflection;
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

    [Header ("Animation")]
    public float animationTime;
    public Image linesImg;
    public Image borderImg, selectedImg;
    public Color selectedMainColor, unselectedMainColor;

    public Color selectedTextColor, unselectedTextColor;

    // Private Variables
    private Vector3 _velocity;
    private bool _hovered;
    private bool _selected;
    private CarObject _car;
    private float _animationTimer;

    private void Awake ( )
    {
        animator = GetComponent<Animator> ( );
    }

    private void Update ( )
    {
        Vector3 target = _hovered ? hoveredScale : unhoveredScale;
        carImg.transform.localScale = Vector3.SmoothDamp (carImg.transform.localScale, target, ref _velocity, hoverSmoothTime);

        UpdateAnimation ( );
    }

    void UpdateAnimation ( )
    {
        _animationTimer += Time.deltaTime;

        float progress = Mathf.InverseLerp (0, animationTime , _animationTimer);

        Animate(linesImg,selectedMainColor,unselectedMainColor, _selected);
        Animate(borderImg,selectedMainColor,unselectedMainColor, _selected);
        Animate(selectedImg,selectedMainColor,unselectedMainColor, _selected);
        Animate(selectText,selectedTextColor,unselectedTextColor, _selected);

        void Animate ( Graphic graphic, Color a, Color b, bool reverse = false )
        {
            graphic.color = Color.Lerp (reverse ? b : a, reverse ? a : b, progress);
        }
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
        Debug.Log ("TODO: Add Selected Ainanmtion", this);
        _hovered = hovererd;

        animator.SetBool ("Selected", hovererd);

    }

    public void SetSelected ( bool selected )
    {
        _selected = selected;
        _animationTimer = 0;
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
