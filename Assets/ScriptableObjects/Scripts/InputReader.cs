using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "InputReader", fileName = "InputReader")]
public class InputReader : ScriptableObject, GameControls.IControlsActions
{
    public event UnityAction<Vector2> OnmoveEvent;
    public event UnityAction OnJumpEvent;
    public event UnityAction OnJumpCanceledEvent;
    public event UnityAction OnDashUpEvent;
    public event UnityAction OnDashDownEvent;
    public event UnityAction OnDashLeftEvent;
    public event UnityAction OnDashRightEvent;
    public event UnityAction openMenuEvent;
    private GameControls controls;
    void OnEnable()
    {
        if(controls == null)
        {
            controls = new GameControls();
            controls.Controls.SetCallbacks(this);
        }
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
    public void OnOpenMENU(InputAction.CallbackContext context)
    {
        openMenuEvent?.Invoke();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        OnmoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(OnJumpEvent != null && context.performed)
            OnJumpEvent?.Invoke();
        
        if(OnJumpCanceledEvent != null && context.canceled)
            OnJumpCanceledEvent?.Invoke();
    }
    public void OnDashUp(InputAction.CallbackContext context)
    {
        OnDashUpEvent?.Invoke();
    }
    public void OnDashDown(InputAction.CallbackContext context)
    {
        OnDashDownEvent?.Invoke();
    }
    public void OnDashLeft(InputAction.CallbackContext context)
    {
        OnDashLeftEvent?.Invoke();
    }
    public void OnDashRight(InputAction.CallbackContext context)
    {
        OnDashRightEvent?.Invoke();
    }
}
