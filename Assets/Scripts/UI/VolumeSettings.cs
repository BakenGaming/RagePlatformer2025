using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    private float masterLoad, musicLoad, sfxLoad;

    public void Initialize()
    {
        if(PlayerPrefs.HasKey("MasterVolume")) InitialVolumeLoad();
    }

    public void SettingsMenuOpened()
    {
        if(PlayerPrefs.HasKey("MasterVolume")) LoadVolumes();
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    private void InitialVolumeLoad()
    {
        masterLoad = PlayerPrefs.GetFloat("MasterVolume");
        mainMixer.SetFloat("Master", Mathf.Log10(masterLoad)*20f);  

        musicLoad = PlayerPrefs.GetFloat("MusicVolume");
        mainMixer.SetFloat("Music", Mathf.Log10(musicLoad)*20f); 
        
        sfxLoad = PlayerPrefs.GetFloat("SFXVolume");
        mainMixer.SetFloat("SFX", Mathf.Log10(sfxLoad)*20f); 
    }

    public void SetMasterVolume()
    {
        float volume = masterVolumeSlider.value;
        mainMixer.SetFloat("Master", Mathf.Log10(volume)*20f);  
        PlayerPrefs.SetFloat("MasterVolume", volume);      
    }
    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        mainMixer.SetFloat("Music", Mathf.Log10(volume)*20f); 
        PlayerPrefs.SetFloat("MusicVolume", volume);       
    }
    public void SetSFXVolume()
    {
        float volume = SFXVolumeSlider.value;
        mainMixer.SetFloat("SFX", Mathf.Log10(volume)*20f);   
        PlayerPrefs.SetFloat("SFXVolume", volume);     
    }

    private void LoadVolumes()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

}
