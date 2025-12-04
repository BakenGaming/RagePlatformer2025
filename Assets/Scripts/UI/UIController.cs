using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour
{
    #region Variables
    [Header("MENUS")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject dialogScreen;
    [SerializeField] private bool isMainMenu;
    [Header("GAME UI")]
    [SerializeField] private GameObject mainGameUI;
    //[Header("MISC UI")]
    #endregion
    #region Setup
    private void OnEnable() 
    {
        if (isMainMenu) Initialize();
    }
    private void OnDisable() 
    {

    }
    private void Initialize()
    {
        GetComponent<VolumeSettings>().Initialize();        
        if(!isMainMenu) pauseMenu.SetActive(false);
        else creditsScreen.SetActive(false);
        
        settingsMenu.SetActive(false);
    }
    #endregion
    #region UI Menu Controls
    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        GameManager.i.PauseGame();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);
        GameManager.i.UnPauseGame();
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        GetComponent<VolumeSettings>().SettingsMenuOpened();
    }

    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }

    public void OpenCreditsScreen()
    {
        creditsScreen.SetActive(true);
    }

    public void CloseCreditsScreen()
    {
        creditsScreen.SetActive(false);
    }
    public void EnableMainGameUI()
    {
        mainGameUI.SetActive(true);
    }
    public void DisableMainGameUI()
    {
        mainGameUI.SetActive(false);
    }
    public void OpenDialogMenu()
    {
        dialogScreen.SetActive(true);
        DialogHandler.i.StartDialog();
    }
    public void CloseDialogMenu()
    {
        dialogScreen.SetActive(false);
    }

    #endregion
    #region Scene Control
    public void StartGame()
    {
        SceneController.StartGame();
    }

    public void RestartGame()
    {
        SceneController.StartGame();
    }
    public void BackToMainMenu()
    {
        SceneController.LoadMainMenu();
    }

    public void ExitGame()
    {
        SceneController.ExitGame();
    }
    #endregion
}
