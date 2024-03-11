using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;

public class OptionsMenu : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField]TMP_Dropdown ResolutionDD;
    [SerializeField] Toggle FullScreenToggle;

    Resolution[] availableResolutions;


    void Awake()
    {
        InitialiseGraphicsElements();
    }

    public void SetResolution(int i)
    {
        Resolution r = availableResolutions[i];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }
    
    public void SetFullScreen(bool fullscreen) => Screen.fullScreen = fullscreen;

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
