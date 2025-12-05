using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    [Range(1f, 100f)] public float maxRunSpeed = 20f;

    [Header("Ground/Collision Checks")]
    public float groundDetectionRayLength = .02f;
    public float headDetectionRayLength = .02f;
    [Range(0f,1f)] public float headWidth = .75f;
    public bool debugShowIsGroundedBox = true;
    public bool debugShowHeadBumpBox = true;

    public float jumpPower;
    public float dashAmount;
}
