using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;
using System;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] TMP_Dropdown DifficultyDD;

    [Header("Graphics")]
    [SerializeField]TMP_Dropdown ResolutionDD;
    [SerializeField] Toggle FullScreenToggle;

    Resolution[] availableResolutions;

    [Header("Audio")]
    [SerializeField] AudioMixer Mixer;
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider MusicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider UIFXSlider;


    void Awake()
    {
        InitialiseGraphicsElements();
        InitialiseAudioElements();
        InitialiseDifficulty();
    }

    public void SetResolution(int i)
    {
        Resolution r = availableResolutions[i];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }
    
    public void SetFullScreen(bool fullscreen) => Screen.fullScreen = fullscreen;

    public void SetDifficulty(int difficulty) => PlayerPrefs.SetInt("Difficulty", difficulty);

    public void SetMasterVolume(float v) => Mixer.SetFloat("MasterVolume", v);
    public void SetSFXVolume(float v) => Mixer.SetFloat("SFXVolume", v);
    public void SetUIFXVolume(float v) => Mixer.SetFloat("UIFXVolume", v);
    public void SetMusicVolume(float v) => Mixer.SetFloat("MusicVolume", v);

    void InitialiseGraphicsElements()
    {
        //Find options for resolution.
        availableResolutions = Screen.resolutions;
        List<string> resOptions = availableResolutions.
                                    Select(res => res.width.ToString() + " x " + res.height.ToString()).
                                    ToList();
        ResolutionDD.ClearOptions(); ResolutionDD.AddOptions(resOptions);
        //Use recommended resolution if possible!
        SetResolution(ClosestResToHD());
        ResolutionDD.SetValueWithoutNotify(Array.IndexOf(availableResolutions, Screen.currentResolution));

        FullScreenToggle.isOn = Screen.fullScreen;
    }

    void InitialiseAudioElements()
    {
        float f = 0f;
        Mixer.GetFloat("MasterVolume", out f); MasterSlider.value = f;
        Mixer.GetFloat("SFXVolume", out f); SFXSlider.value = f;
        Mixer.GetFloat("UIFXVolume", out f); UIFXSlider.value = f;
        Mixer.GetFloat("MusicVolume", out f); MusicSlider.value = f;
    }

    void InitialiseDifficulty()
    {
        int setting = PlayerPrefs.HasKey("Difficulty") ? PlayerPrefs.GetInt("Difficulty") : 1;
        DifficultyDD.SetValueWithoutNotify(setting);
    }

    int ClosestResToHD()
    {
        int minDist = Int32.MaxValue;
        int result = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution r = availableResolutions[i];
            int dist = (r.width - 1920) * (r.width - 1920)  +  (r.height - 1080) * (r.height - 1080);
            if (dist < minDist)
            {
                minDist = dist;
                result = i;
            }
        }
        return result;
    }
}
