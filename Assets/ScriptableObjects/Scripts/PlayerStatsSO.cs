using UnityEngine;

[CreateAssetMenu(menuName ="Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
    [Range(.25f, 50f)] public float groundAcceleration = 5f;
    [Range(.25f, 50f)] public float groundDeceleration = 20f;
    [Range(.25f, 50f)] public float airAcceleration = 5f;
    [Range(.25f, 50f)] public float airDeceleration = 5f;
    
    [Header("Run")]
    public bool playerCanRun = false;
    [Range(1f, 100f)] public float maxRunSpeed = 20f;

    [Header("Ground/Collision Checks")]
    public float groundDetectionRayLength = .02f;
    public float headDetectionRayLength = .02f;
    [Range(0f,1f)] public float headWidth = .75f;

    [Header("Jump")]
    [Range(2f, 10f)] public float jumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = .35f;
    [Range(.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range(1, 2)] public int numberOfJumpsAllowed = 1;

    [Header("Jump Cut")]
    [Range(.02f, .3f)] public float timeForUpwardsCancel = .027f;

    [Header("Jump Apex")]
    [Range(.5f, 1f)] public float apexThreshold = .97f;
    [Range(.01f, 1f)] public float apexHangTime = .075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = .125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = .1f;

    [Header("Debug")]
    public bool debugShowIsGroundedBox = true;
    public bool debugShowHeadBumpBox = true;

    [Header("Jump Visualization Tool")]
    public bool showWalkJumpArc = false;
    public bool showRunJumpArc = false;
    public bool stopOnCollision = true;
    public bool drawRight = true;
    [Range(5,100)] public int arcResolution = 20;
    [Range(0,500)] public int visualizationSteps = 90;
    public float gravity {get; private set;}
    public float initialJumpVelocity {get; private set;}
    public float adjustedJumpHeight {get; private set;}

    [Header("Dash")]
    public bool playerCanDash = false;
    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }
    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillJumpApex;
    }
}
