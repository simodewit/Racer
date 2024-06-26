using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SizeSelector : OptionInputReceiver
{
    // Fields
    [Header ("Settings")]
    [SerializeField]
    private int startIndex;
    [SerializeField]
    private Transform parent;

    [Header ("Animation")]
    public float animationTime;
    public Color selectedBackgroundColor;
    public Color unselectedBackgroundColor;
    public Color selectedTextColor;
    public Color unselectedTextColor;
    public float selectedHeight;
    public float unselectedHeight;

    public string[] options = new string[] { "Option 1", "Option 2", "Option 3" };

    public UnityEvent<int> onToggleChanged;

    [Header ("References")]
    public GameObject togglePrefab;

    // Private Variables

    private float _animationTimer;
    [SerializeField]
    private int _currentIndex;

    private ToggleElement[] _elements;

    // Properties

    public int CurrentIndex
    {
        get
        {
            return _currentIndex;
        }
        private set
        {
            _currentIndex = value;

            OnIndexChanged ( );
        }
    }

    // Unity Messages

    private void Start ( )
    {
        InitializeElements ( );
    }

    private void Update ( )
    {
        HandleAnimations ( );
    }

    // Methods

    private void InitializeElements ( )
    {
        _elements = new ToggleElement[options.Length];
        for ( int i = 0; i < options.Length; i++ )
        {
            ToggleElement element = new ToggleElement ( );

            GameObject prefab = Instantiate (togglePrefab, parent);

            element.transform = prefab.GetComponent<RectTransform> ( );
            element.textElement = prefab.GetComponentInChildren<TextMeshProUGUI> ( );
            element.background = prefab.GetComponent<Image> ( );

            element.textElement.text = options[i];

            bool selected = i == startIndex;

            float heigth = selected ? selectedHeight : unselectedHeight;
            Color textColor = selected ? selectedTextColor : unselectedTextColor;
            Color bgColor = selected ? selectedBackgroundColor : unselectedBackgroundColor;

            var size = element.transform.sizeDelta;
            element.transform.sizeDelta = new Vector2 (size.x, heigth);

            element.textElement.color = textColor;
            element.background.color = bgColor;

            _elements[i] = element;
        }
    }

    private void HandleAnimations ( )
    {
        _animationTimer += Time.deltaTime;

        float progress = Mathf.InverseLerp (0, animationTime, _animationTimer);

        for ( int i = 0; i < _elements.Length; i++ )
        {
            bool selected = i == CurrentIndex;

            var element = _elements[0];

            element.background.color = Color.Lerp (element.background.color, selected ? selectedBackgroundColor : unselectedBackgroundColor, progress);

            element.textElement.color = Color.Lerp (element.textElement.color, selected ? selectedTextColor : unselectedTextColor, progress);

            var size = element.transform.sizeDelta;
            element.transform.sizeDelta = new Vector2 (size.x, Mathf.Lerp (size.x, selected ? selectedHeight : unselectedHeight, progress));

        }
    }

    private void OnIndexChanged ( )
    {
        _animationTimer = 0;

        onToggleChanged.Invoke (_currentIndex);
    }

    // Input

    public override void OnReceiveHorizontalInput ( System.Single input )
    {
        if ( input < 0 && _currentIndex > 0 )
            CurrentIndex--;
        else if ( input > 0 && _currentIndex < options.Length)
            CurrentIndex++;
    }

    public void SetIndex(int index )
    {
        CurrentIndex = Mathf.Clamp (index, 0, options.Length);
    }


    // Classes

    [System.Serializable]
    public class ToggleElement
    {
        public Image background;
        public RectTransform transform;
        public TextMeshProUGUI textElement;
    }
}
