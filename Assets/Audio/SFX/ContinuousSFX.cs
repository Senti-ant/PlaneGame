using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource Source;
    [SerializeField] AudioSource Copy;
    [SerializeField] AudioClip Sounds;

    [Header("Anti-Loopification")]
    [SerializeField] float MinContinuousLength;
    [SerializeField] float MaxContinuousLength;
    [SerializeField] float FadeTime;

    [Header("Controls")]
    [SerializeField] bool PlayOnStart;

    float baseVolume;

    // Start is called before the first frame update
    void Start()
    {
        if (Sounds == null) Sounds = Source.clip;

        Source.clip = Sounds;  Copy.clip = Sounds;
        Source.loop = true;                  Copy.loop = true;
        baseVolume = Source.volume;

        if (PlayOnStart)
            Play();
    }

    public void Play()
    {
        Source.volume = baseVolume;
        Copy.volume = 0f;
        Source.time = Random.Range(0f, Source.clip.length - 0.1f);
        Source.Play();

        StartCoroutine(Deloopification());
    }

    public void Stop()
    {
        StopAllCoroutines();
        Source.Stop(); Copy.Stop();
    }

    IEnumerator Deloopification()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinContinuousLength, MaxContinuousLength));
            yield return FadeFromTo(Source, Copy);
            yield return new WaitForSeconds(Random.Range(MinContinuousLength, MaxContinuousLength));
            yield return FadeFromTo(Copy, Source);
        }
    }

    IEnumerator FadeFromTo(AudioSource from, AudioSource to) => Tween.Routine
    (
        Tween.WithRange(0f, baseVolume, Tween.Linear),
        x => { from.volume = baseVolume - x; to.volume = x; },
        FadeTime
    );
}
