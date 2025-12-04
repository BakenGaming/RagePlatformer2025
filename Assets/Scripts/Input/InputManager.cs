using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _i;
    public static InputManager i { get { return _i; } }

    //Setup inputs
    public Vector2 moveInput {get; private set;}
    public bool menuOpenInput {get; private set;}
    private bool isActive;

    private PlayerInput _playerInput;
    private InputAction _move, _menu;
    public void Initialize()
    {
        _i = this;
        _playerInput = GetComponent<PlayerInput>();

        _move = _playerInput.actions["Move"];
        _menu = _playerInput.actions["MenuOpen"];

        GameManager.i.UnPauseGame();
        isActive = true;
    }

    void Update()
    {
        if(!isActive) return;

        moveInput = _move.ReadValue<Vector2>();
        menuOpenInput = _menu.WasPressedThisFrame();
    }
}
