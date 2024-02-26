using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the hand animation
/// </summary>
public class HandAnimation : MonoBehaviour {
    
    // Inputs action properties
    [SerializeField] private InputActionProperty gripInputActionProperty;
    private InputAction GripInputAction => gripInputActionProperty.action;
    [SerializeField] private InputActionProperty triggerInputActionProperty;
    private InputAction TriggerInputAction => triggerInputActionProperty.action;

    // Animator
    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
        BindHandEvent(GripInputAction);
        BindHandEvent(TriggerInputAction);
    }

    /// <summary>
    /// Bind input events with the animator
    /// </summary>
    /// <param name="inputAction"></param>
    private void BindHandEvent(InputAction inputAction) {
        if(!inputAction.enabled) inputAction.Enable();
        inputAction.started += context => UpdateHandPose();
        inputAction.performed += context => UpdateHandPose();
        inputAction.canceled += context => UpdateHandPose();
    }

    /// <summary>
    /// Update the animator state
    /// </summary>
    private void UpdateHandPose() {
        _animator.SetFloat("Trigger", TriggerInputAction.ReadValue<float>());
        _animator.SetFloat("Grip", GripInputAction.ReadValue<float>());
    }
}
