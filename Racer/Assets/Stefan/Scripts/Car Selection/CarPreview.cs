using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarPreview : MonoBehaviour
{
    [Header ("Settings")]
    public Vector3 rotationSpeed;
    public Vector3 minZoom, maxZoom;
    public Vector2 moveBounds;
    public float moveSpeed;

    public Vector3 defaultAngles;

    [Header ("References")]
    public Transform cameraPivot;
    public Transform spawnPoint;
    public MainMenuManager mainMenu;

    //Private Variables

    [SerializeField]
    private float _zoomProgress;
    [SerializeField]
    private bool _active;

    public void SetPrefab ( GameObject prefab )
    {
        if ( prefab == null )
            return;

        ResetView ();

        //Destroy current previewed object

        if(spawnPoint.childCount > 0 )
        {
            Destroy (spawnPoint.GetChild (0).gameObject);
        }

        Instantiate (prefab, spawnPoint);
    }

    public void SetActive(bool isActive )
    {
        _active = isActive;
    }

    public void RotationInput ( InputAction.CallbackContext context)
    {
        if ( !_active )
            return;
        
        transform.localEulerAngles += context.ReadValue<Vector2>().x * Time.deltaTime * rotationSpeed;
    }

    #region Zooming (Obsolete)
    //public void ZoomInput ( InputAction.CallbackContext context )
    //{
    //    return;

    //    if ( !_active )
    //        return;

    //    _zoomProgress += Time.deltaTime * context.ReadValue<Vector2> ( ).x;

    //    cameraPivot.position = Vector3.Lerp (minZoom, maxZoom, _zoomProgress);
    //}

    //public void MoveInput(InputAction.CallbackContext context )
    //{
    //    return;

    //    if ( !_active )
    //        return;

    //    Vector3 pos = cameraPivot.position;

    //    Vector2 input = context.ReadValue<Vector2> ( );

    //    pos.x += input.x * Time.deltaTime * moveSpeed;
    //    pos.z += input.y * Time.deltaTime * moveSpeed;

    //    pos.x = Mathf.Clamp (pos.x, -moveBounds.x, moveBounds.x);
    //    pos.y = Mathf.Clamp (pos.y, -moveBounds.y, moveBounds.y);

    //    cameraPivot.position = pos;
    //}

    #endregion

    public void ResetView ( )
    {
        if ( !_active )
            return;

        transform.eulerAngles = defaultAngles;

        ResetCameraOffset ( );
    }


    public void ResetCameraOffset ( )
    {
        cameraPivot.transform.localPosition = Vector3.zero;
    }
}
