using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHandler : MonoBehaviour, IHandler
{
    #region Events
    public event UnityAction OnPlayerDeath;
    public event UnityAction OnPlayerInitialized;
    #endregion
    #region Variables
    [Header("References")]
    public PlayerStatsSO Stats;
    public InputReader Input;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;
    private Rigidbody2D thisRB;

    //Movement Variables
    private Vector2 moveVelocity, moveInputVector;
    private bool facingRight=true;

    //Collision Checks
    private RaycastHit2D groundHit, headHit;
    private bool isFacingRight, isGrounded, bumpedHead, isRunning, isDashing;
    #endregion
    #region Initialize
    void OnDisable()
    {
        Input.OnMoveEvent -= OnMove;
        Input.OnRunEvent -= OnRun;
        Input.OnRunCanceledEvent -= OnRunCanceled;
        Input.OnDashEvent -= OnDash;
        Input.OnDashCanceled -= OnDashCanceled;
        Input.OnJumpEvent -= OnJump;
        Input.OnJumpCanceledEvent -= OnJumpCanceled;
    }
    //Remove after Testing
    void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        thisRB = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        Input.OnMoveEvent += OnMove;
        Input.OnRunCanceledEvent += OnRunCanceled;
        Input.OnDashEvent -= OnDash;
        Input.OnDashCanceled += OnDashCanceled;
        Input.OnJumpEvent += OnJump;
        Input.OnJumpCanceledEvent += OnJumpCanceled;
        OnPlayerInitialized?.Invoke();
    }
    #endregion
    #region Movement Triggers
    private void OnMove(Vector2 movement)
    {
        moveInputVector = movement;
    }
    private void OnRun()
    {
        isRunning = true;
    }
    private void OnRunCanceled()
    {
        isRunning = false;
    }
    private void OnDash()
    {
        isDashing = true;
    }
    private void OnDashCanceled()
    {
        isDashing = false;
    }
    private void OnJump()
    {
        
    }

    private void OnJumpCanceled()
    {
        
    }
    #endregion
    #region Player Functions
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if(moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);
            Vector2 targetVelocity = Vector2.zero;
            
            if(isRunning) targetVelocity = new Vector2(moveInput.x, 0f) * Stats.maxRunSpeed;
            else targetVelocity = new Vector2(moveInput.x, 0f) * Stats.maxWalkSpeed;

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.deltaTime);
            thisRB.linearVelocity = new Vector2(moveVelocity.x, thisRB.linearVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.deltaTime);
            thisRB.linearVelocity = new Vector2(moveVelocity.x, thisRB.linearVelocity.y);
        }
    }
    void FixedUpdate()
    {
        CollisionCheck();
        if(isGrounded) Move(Stats.groundAcceleration, Stats.groundDeceleration, moveInputVector);
        else Move(Stats.airAcceleration, Stats.airDeceleration, moveInputVector);
    }
    public void TriggerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    #endregion
    #region Checks
    private void Grounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, Stats.groundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, 
            Vector2.down, Stats.groundDetectionRayLength, StaticVariables.i.GetGroundLayer());
        
        if(groundHit.collider != null) isGrounded = true;
        else isGrounded = false;

        #region Debug Visualization
        if(Stats.debugShowIsGroundedBox)
        {
            Color rayColor;
            if(isGrounded) rayColor = Color.green;
            else rayColor = Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector3.down * Stats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector3.down * Stats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - Stats.groundDetectionRayLength), Vector3.right * boxCastSize, rayColor);
        }
        #endregion
    }
    private void CollisionCheck()
    {
        Grounded();
    }
    private void TurnCheck(Vector2 moveInput)
    {
        if(isFacingRight && moveInput.x < 0) Turn(false);
        else if (!facingRight && moveInput.x > 0) Turn(true);
        
    }
    private void Turn(bool turnRight)
    {
        if (!turnRight)
        {
            thisRB.transform.localScale = new Vector3(-1f, 1f, 1f);
            facingRight = false;
        }
        else
        {
            thisRB.transform.localScale = Vector3.one;
            facingRight = true;
        }
    }
    #endregion
    #region Get Functions
    #endregion
}
