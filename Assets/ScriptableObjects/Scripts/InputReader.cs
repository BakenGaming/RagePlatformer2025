using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "InputReader", fileName = "InputReader")]
public class InputReader : ScriptableObject
{
    public event UnityAction<Vector2> OnMoveEvent;
    public event UnityAction OnJumpEvent;
    public event UnityAction OnJumpCanceledEvent;
    public event UnityAction OnDashEvent;
    public event UnityAction OnDashCanceled;
    public event UnityAction OnRunEvent;
    public event UnityAction OnRunCanceledEvent;
    public event UnityAction OnMenuOpenEvent;

    [SerializeField] private InputActionAsset _asset;
    private InputAction _move, _jump, _dash, _run, _menu;

    void OnEnable()
    {
        _move = _asset.FindAction("MoveInput");
        _jump = _asset.FindAction("Jump");
        _dash = _asset.FindAction("Dash");
        _run = _asset.FindAction("Run");
        //_menu = _asset.FindAction("OpenMenu");

        _move.started += OnMoveInput;
        _move.performed += OnMoveInput;
        _move.canceled += OnMoveInput;
        
        _jump.started += OnJump;
        _jump.performed += OnJump;
        _jump.canceled += OnJump;
        
        _dash.started += OnDash;
        _dash.performed += OnDash;
        _dash.canceled += OnDash;

        _run.started += OnRun;
        _run.performed += OnRun;
        _run.canceled += OnRun;
        
        _move.Enable();
        _jump.Enable();
        _dash.Enable();
        _run.Enable();
    }

    void OnDisable()
    {
        _move.started -= OnMoveInput;
        _move.performed -= OnMoveInput;
        _move.canceled -= OnMoveInput;
        
        _jump.started -= OnJump;
        _jump.performed -= OnJump;
        _jump.canceled -= OnJump;

        _dash.started -= OnDash;
        _dash.performed -= OnDash;
        _dash.canceled -= OnDash;

        _run.started -= OnRun;
        _run.performed -= OnRun;
        _run.canceled -= OnRun;

        _move.Disable();
        _jump.Disable();
        _dash.Disable();
        _run.Disable();
    }
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(OnJumpEvent != null && context.started)
        {
            OnJumpEvent?.Invoke();
        }

        if(OnJumpCanceledEvent != null && context.canceled)
        {
            OnJumpCanceledEvent?.Invoke();
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if(OnDashEvent != null && context.started)
        {
            OnDashEvent?.Invoke();
        }

        if(OnDashCanceled != null && context.canceled)
        {
            OnDashCanceled?.Invoke();
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if(OnRunEvent != null && context.started)
        {
            OnRunEvent?.Invoke();
        }

        if(OnRunCanceledEvent != null && context.canceled)
        {
            OnRunCanceledEvent?.Invoke();
        }
    }
    public void OnOpenMENU(InputAction.CallbackContext context)
    {
    }
}
