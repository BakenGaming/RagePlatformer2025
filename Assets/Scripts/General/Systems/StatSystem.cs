using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSystem
{
    private float moveSpeed;
    private float jumpPower;

    public StatSystem (PlayerStatsSO _stats)
    {
        moveSpeed = _stats.moveSpeed;
        jumpPower = _stats.jumpPower;

    }
    public float GetMoveSpeed(){return moveSpeed;}
    public float GetJumpPower(){return jumpPower;}
}
