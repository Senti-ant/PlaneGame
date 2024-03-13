using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Radar : MonoBehaviour
{
    [SerializeField] Light2D RadarLight;
    [SerializeField] float PulseLength;
    [SerializeField] float PulseCooldown;

    float lastPulseTime;

    void Start() => lastPulseTime = -PulseCooldown;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && Time.timeSinceLevelLoad > lastPulseTime + PulseCooldown)
        {
            lastPulseTime = Time.timeSinceLevelLoad;

            GetComponent<RandomSFX>().Play();
            StartCoroutine(SendRadarPulse());
        }
    }

    IEnumerator SendRadarPulse() => Tween.Routine
    (
        Tween.PingPong(Tween.Linear),
        x => RadarLight.intensity = x,
        PulseLength
    );
}
