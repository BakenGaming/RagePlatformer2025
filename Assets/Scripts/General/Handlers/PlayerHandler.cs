using UnityEngine;
using UnityEngine.Events;

public class PlayerHandler : MonoBehaviour, IHandler
{
    #region Events
    public event UnityAction OnPlayerDeath;
    #endregion
    #region Variables
    public InputReader Input;
    public PlayerStatsSO PlayerStatsSO;
    #endregion
    #region Initialize
    public void Initialize()
    {
        GetComponent<PlayerMovementHandler>().Initialize(this);
    }
    #endregion
    #region Handle Player Functions
    public void TriggerDeath()
    {
        OnPlayerDeath?.Invoke();
    }
    #endregion
    #region Get Functions
    #endregion
}
