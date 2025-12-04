using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementHandler : MonoBehaviour
{
    public event UnityAction OnPlayerInitialized;
    private InputReader _input;
    private PlayerStatsSO _stats;
    private Rigidbody2D thisRB;
    private Vector2 moveInput;

    void OnDisable()
    {
        _input.OnmoveEvent += OnMove;
        _input.OnJumpEvent += OnJump;
        _input.OnJumpCanceledEvent += OnJumpCanceled;
    }

    public void Initialize(PlayerHandler _handler)
    {
        thisRB = GetComponent<Rigidbody2D>();
        _input = GetComponent<InputReader>();
        _stats = _handler.PlayerStatsSO;
        _input.OnmoveEvent += OnMove;
        _input.OnJumpEvent += OnJump;
        _input.OnJumpCanceledEvent += OnJumpCanceled;
        OnPlayerInitialized?.Invoke();
    }
    private void OnMove(Vector2 movement)
    {
        moveInput = movement.normalized;
    }

    private void OnJump()
    {
        
    }

    private void OnJumpCanceled()
    {
        
    }

    void LateUpdate()
    {
        thisRB.linearVelocity = moveInput * _stats.moveSpeed * Time.deltaTime;
    }
}
