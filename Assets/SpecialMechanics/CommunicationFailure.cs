using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommunicationFailure : MonoBehaviour
{
    [Header("What is and isn't visible")]
    [SerializeField] LayerMask VisibleWhenNoOutage;
    [SerializeField] LayerMask AlwaysVisible;
    [SerializeField] Image OutageImage;

    [Header("Outage parameters")]
    [SerializeField] int MinNumOutages;
    [SerializeField] int MaxNumOutages;
    [SerializeField] float MinOutageLength;
    [SerializeField] float MaxOutageLength;
    [SerializeField] float MinOutageTime;
    [SerializeField] float MaxOutageTime;

    Camera mainCam;
    float[] OutageTimes;

    void Start()
    {
        mainCam = Camera.main;

        OutageTimes = new float[UnityEngine.Random.Range(MinNumOutages, MaxNumOutages)];
        for (int i = 0; i < OutageTimes.Length; i++)
            OutageTimes[i] = UnityEngine.Random.Range(MinOutageTime, MaxOutageTime);
        Array.Sort(OutageTimes);

        StartCoroutine(Outages());
    }

    IEnumerator Outages()
    {
        for (int i = 0; i < OutageTimes.Length; i++)
        {
            float nextStartTime = OutageTimes[i];
            yield return new WaitUntil(() => Time.timeSinceLevelLoad > nextStartTime);
            DisableCommunications();

            float length = UnityEngine.Random.Range(MinOutageLength, MaxOutageLength);
            yield return new WaitUntil(() => Time.timeSinceLevelLoad > nextStartTime + length);

            //Edge case: Next outage has already started by the time this one is done.
            if (i < OutageTimes.Length - 1 && Time.timeSinceLevelLoad > OutageTimes[i+1])
                continue;
            else
                EnableCommunications();
        }
    }

    IEnumerator TemporaryOutageEffect() =>
        Tween.Routine
        (
            Tween.PingPong(Tween.Linear),
            x => OutageImage.color = new Color(OutageImage.color.r, OutageImage.color.g, OutageImage.color.b, x),
            0.1f
        );

    void DisableCommunications() 
    {
        //Avoid redundant calls: these might cause weird bugs down the line if/when we add VFX and stuff.
        if (mainCam.cullingMask == AlwaysVisible)
            return;

        StartCoroutine(TemporaryOutageEffect());
        mainCam.cullingMask = AlwaysVisible;
    }

    void EnableCommunications()
    {
        //Avoid redundant calls: these might cause weird bugs down the line if/when we add VFX and stuff.
        if (mainCam.cullingMask == VisibleWhenNoOutage)
            return;

        StartCoroutine(TemporaryOutageEffect());
        mainCam.cullingMask = VisibleWhenNoOutage;
    }
}
