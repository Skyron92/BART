using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Manages the pump handle behaviour, including grab behaviour
/// </summary>
public class PumpHandle : MonoBehaviour
{
    // The start position of the handle
    private Vector3 _initialPosition;
    // The speed the handle reset it position
    [SerializeField, Range(0f, 1f)] private float resetPositionSpeed;
    // Allows to know if the ResetPosition must stop
    private bool IsAtInitialPosition => Vector3.Distance(transform.position, _initialPosition) < 0.01f;
    
    // The calculated position set to the current position of the handle
    private Vector3 _position;
    
    // The bottom limit transform of the handle
    [SerializeField] private Transform clampMin;
    // The bottom limit position of the handle
    private Vector3 PosMin => clampMin.position;
    // The Y values of the top and bottom limits positions
    private float Ymin => PosMin.y;
    private float Ymax => _initialPosition.y;

    // The XRSimpleInteractable component attached to the handle
    private XRSimpleInteractable _xrSimpleInteractable;
    // The hand interacting with the handle
    private IXRSelectInteractor XRSelectInteractor => _xrSimpleInteractable.interactorsSelecting[0];
    // The BaseInteractionEventArgs, allows to trigger exit events of the Simple interactable component
    private HoverExitEventArgs _hoverExitEventArgs = new HoverExitEventArgs();
    private SelectExitEventArgs _selectExitEventArgs = new SelectExitEventArgs();

    // The materials of the handle
    [SerializeField] private Material hoveredMaterial;
    [SerializeField] private Material selectedMaterial;

    // The distance where the handle stop to follow the hand
    [SerializeField, Range(0f, 1f)] private float hoverExitRange;

    // Reference to the balloon
    [SerializeField] private Balloon balloonReference;

    private void Awake() {
        // Set properties
        _xrSimpleInteractable = GetComponent<XRSimpleInteractable>();
        _initialPosition = transform.position;
        _position = transform.position;
        SetUpBaseInteractionEventArgs(_hoverExitEventArgs, _selectExitEventArgs);
        _hoverExitEventArgs.manager = _xrSimpleInteractable.interactionManager;
        _selectExitEventArgs.manager = _xrSimpleInteractable.interactionManager;
    }

    private void Update() {
        // Tracks hand
        if(_xrSimpleInteractable.isSelected) StartCoroutine(FollowHand());
        // Reset position
        else if(!IsAtInitialPosition) transform.position = Vector3.Lerp(transform.position, _initialPosition, resetPositionSpeed);
    }

    /// <summary>
    /// Makes the handle track the interacting hand
    /// </summary>
    /// <returns>The delay</returns>
    private IEnumerator FollowHand() {
        if (HandIsTooFar()) {
            _xrSimpleInteractable.selectExited.Invoke(_selectExitEventArgs);
            _xrSimpleInteractable.hoverExited.Invoke(_hoverExitEventArgs);
            yield break;
        }
        if (balloonReference.IsInflating) yield return new WaitForSeconds(0.1f);
        else {
            if(_position.y >= Ymin && _position.y <= Ymax) _position.y = XRSelectInteractor.transform.position.y;
            if (_position.y > Ymax) _position.y = Ymax;
            if (_position.y < Ymin) _position.y = Ymin;
            transform.position = _position;
        }
        yield return new WaitForSeconds(0.1f);
    }

    /// <summary>
    /// Calculates the distance between the handle and the interacting hand
    /// </summary>
    /// <returns>True if the hand is too far to continue grabbing the handle </returns>
    private bool HandIsTooFar() {
        return Vector3.Distance(XRSelectInteractor.transform.position, transform.position) >= hoverExitRange;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hoverExitRange);
    }

    /// <summary>
    /// Attributes the interaction manager to the Base interaction event args
    /// </summary>
    /// <param name="args0"></param>
    /// <param name="args1"></param>
    public void SetUpBaseInteractionEventArgs(BaseInteractionEventArgs args0, BaseInteractionEventArgs args1) {
        args0.interactableObject = _xrSimpleInteractable;
        args1.interactableObject = _xrSimpleInteractable;
    }

    private void OnTriggerStay(Collider other) {
        // Reattributes the correct material to the handle
        if (other.CompareTag("GameController")) 
            GetComponent<MeshRenderer>().material = 
                _xrSimpleInteractable.isSelected ? selectedMaterial : hoveredMaterial;
    }
}
