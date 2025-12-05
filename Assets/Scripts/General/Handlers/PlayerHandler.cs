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
    private bool isFacingRight=true, isRunning;

    //Collision Checks
    private RaycastHit2D groundHit, headHit;    
    private bool isGrounded, bumpedHead;

    //Jump Variables
    public float verticalVelocity {get; private set;}
    private bool isJumping, isFastFalling, isFalling, isJumpingPressed, isJumpingCanceled;
    private float fastFallingTime, fastFallReleaseSpeed;
    private int numberOfJumpsUsed;

    //Apex Variables
    private float apexPoint, timePastApexThreshold;
    private bool isPastApexThreshold;

    //Jump Buffer Variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    //Coyote Time Variables
    private float coyoteTimer;

    //Dash
    private bool isDashing, isDashingPressed,  isDashingCanceled;
    #endregion
    #region Initialize
    void OnDisable()
    {
        Input.OnMoveEvent -= OnMove;
        if(Stats.playerCanRun)
        {
            Input.OnRunEvent -= OnRun;
            Input.OnRunCanceledEvent -= OnRunCanceled;
        }
        if(Stats.playerCanDash)
        {
            Input.OnDashEvent -= OnDash;
            Input.OnDashCanceled -= OnDashCanceled;
        }
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
        if(Stats.playerCanRun)
        {
            Input.OnRunEvent += OnRun;
            Input.OnRunCanceledEvent += OnRunCanceled;
        }
        if(Stats.playerCanDash)
        {
            Input.OnDashEvent += OnDash;
            Input.OnDashCanceled += OnDashCanceled;
        }
        Input.OnJumpEvent += OnJump;
        Input.OnJumpCanceledEvent += OnJumpCanceled;
        OnPlayerInitialized?.Invoke();
    }
    #endregion
    #region Movement Triggers From Input Events
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
        if(isDashing && !isDashingCanceled) return;

        isDashingPressed = true;
        isDashingCanceled = false;
    }
    private void OnDashCanceled()
    {
        isDashingPressed = false;
        isDashingCanceled = true;
    }
    private void OnJump()
    {
        if(isJumping && !isJumpingCanceled) return;

        isJumpingPressed = true;
        isJumpingCanceled = false;
    }

    private void OnJumpCanceled()
    {
        isJumpingPressed = false;
        isJumpingCanceled = true;;
    }
    #endregion
    #region Player Functions
    #region Movement
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
    #endregion
    #region Jump
    private void Jump()
    {
        if(isJumping)
        {
            if(bumpedHead) isFastFalling = true;

            if(verticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(Stats.initialJumpVelocity, 0f, verticalVelocity);
                if(apexPoint > Stats.apexThreshold)
                {
                    if(!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if(isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if(timePastApexThreshold < Stats.apexHangTime) verticalVelocity = 0f;
                        else verticalVelocity = -.01f;
                    }
                }
                else
                {
                    verticalVelocity += Stats.gravity * Time.fixedDeltaTime;
                    if(isPastApexThreshold) isPastApexThreshold = false;
                }
            }
            else if (!isFastFalling)
                verticalVelocity += Stats.gravity * Stats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            else if(verticalVelocity < 0f)
                if(!isFalling) isFalling = true;
        }

        if(isFastFalling)
        {
            if(fastFallingTime > Stats.timeForUpwardsCancel)
                verticalVelocity += Stats.gravity * Stats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            else if(fastFallingTime < Stats.timeForUpwardsCancel)
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallingTime / Stats.timeForUpwardsCancel));
            fastFallingTime += Time.fixedDeltaTime;
        }

        if(!isGrounded && !isJumping)
        {
            if(!isFalling) isFalling = true;
            
            verticalVelocity += Stats.gravity * Time.fixedDeltaTime;
        }

        verticalVelocity = Mathf.Clamp(verticalVelocity, -Stats.maxFallSpeed, 50f);
        thisRB.linearVelocity = new Vector3(thisRB.linearVelocity.x, verticalVelocity);
    }
    #endregion
    #region Loop
    void Update()
    {
        UpdateTimers();
        JumpChecks();
    }
    void FixedUpdate()
    {
        CollisionCheck();
        Jump();
        if(isGrounded) Move(Stats.groundAcceleration, Stats.groundDeceleration, moveInputVector);
        else Move(Stats.airAcceleration, Stats.airDeceleration, moveInputVector);
    }
    private void UpdateTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if(isGrounded) coyoteTimer = Stats.jumpCoyoteTime;
        else coyoteTimer -= Time.deltaTime;
    }
    #endregion
    #region Handle Death
    public void TriggerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    #endregion
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
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - Stats.groundDetectionRayLength), Vector3.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * Stats.headWidth, Stats.headDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, 
            Vector2.up, Stats.headDetectionRayLength, StaticVariables.i.GetGroundLayer());
        
        if(headHit.collider != null) bumpedHead = true;
        else bumpedHead = false;

        #region Debug Visualization
        if(Stats.debugShowHeadBumpBox)
        {
            float headWidth = Stats.headWidth;
            Color rayColor;
            if(bumpedHead) rayColor = Color.green;
            else rayColor = Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector3.up * Stats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector3.up * Stats.headDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - (boxCastSize.x / 2) * headWidth, boxCastOrigin.y + Stats.headDetectionRayLength), Vector3.right * boxCastSize.x * headWidth, rayColor);
        }
        #endregion 
    }
    private void JumpChecks()
    {
        #region Jump Pressed
        if(isJumpingPressed && !isJumping)
        {
            isJumping = true;
            isJumpingPressed = false;
            jumpBufferTimer = Stats.jumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }
        #endregion
        #region Jump Released
        if(isJumpingCanceled)
        {
            if(jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }

            if(isJumping && verticalVelocity > 0f)
            {
                if(isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallingTime = Stats.timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }
        #endregion
        #region Initiate Jump
        if(jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if(jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }
        }
        #endregion
        #region Double Jump
        if(jumpBufferTimer > 0f && isJumping && numberOfJumpsUsed < Stats.numberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        else if (jumpBufferTimer > 0f && isFalling && numberOfJumpsUsed < Stats.numberOfJumpsAllowed -1)
        {
            isFastFalling = false;
            InitiateJump(2);
        }
        #endregion
        #region Landed
        if((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallingTime = 0f;
            isPastApexThreshold = false;
            numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
        #endregion
    }
    private void InitiateJump(int _numberOfJumpsUsed)
    {
        if(!isJumping)
        {
            isJumping = true;
        }
        jumpBufferTimer = 0f;
        numberOfJumpsUsed += _numberOfJumpsUsed;
        verticalVelocity = Stats.initialJumpVelocity;
    }
    private void CollisionCheck()
    {
        Grounded();
        BumpedHead();
    }
    private void TurnCheck(Vector2 moveInput)
    {
        if(isFacingRight && moveInput.x < 0) Turn(false);
        else if (!isFacingRight && moveInput.x > 0) Turn(true);
        
    }
    private void Turn(bool turnRight)
    {
        if (!turnRight)
        {
            thisRB.transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = false;
        }
        else
        {
            thisRB.transform.localScale = Vector3.one;
            isFacingRight = true;
        }
    }
    #endregion
    #region Get Functions
    #endregion
    #region Jump Debug Arc
    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 previousPosition = startPosition;
        
        float speed = 0f;
        if(Stats.drawRight) speed = moveSpeed;
        else speed = -moveSpeed;

        Vector2 velocity = new Vector2(speed, Stats.initialJumpVelocity);
        Gizmos.color = gizmoColor;
        float timeStep = Stats.timeTillJumpApex / Stats.arcResolution;

        for (int i = 0; i < Stats.visualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if(simulationTime < Stats.timeTillJumpApex)
                displacement = velocity * simulationTime + .5f * new Vector2(0, Stats.gravity) * simulationTime * simulationTime;
            else if (simulationTime < Stats.timeTillJumpApex + Stats.apexHangTime)
            {
                float apexTime = simulationTime - Stats.timeTillJumpApex;
                displacement = velocity * Stats.timeTillJumpApex + .5f * new Vector2(0, Stats.gravity) * Stats.timeTillJumpApex * Stats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }
            else
            {
                float decendTime = simulationTime - (Stats.timeTillJumpApex + Stats.apexHangTime);
                displacement = velocity * Stats.timeTillJumpApex + .5f * new Vector2(0, Stats.gravity) * Stats.timeTillJumpApex * Stats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * Stats.apexHangTime;
                displacement += new Vector2(speed, 0) * decendTime + .5f * new Vector2(0, Stats.gravity) * decendTime * decendTime;
            }

            drawPoint = startPosition + displacement;
            if(Stats.stopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), StaticVariables.i.GetGroundLayer());
                if(hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
    #endregion
    #region Gizmos
    void OnDrawGizmos()
    {
        if(Stats.showWalkJumpArc) DrawJumpArc(Stats.maxWalkSpeed, Color.green);
        if(Stats.showRunJumpArc) DrawJumpArc(Stats.maxRunSpeed, Color.red);
    }
    #endregion
}
