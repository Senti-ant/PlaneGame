using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class RandomSFX : MonoBehaviour
{
    AudioSource Source;
    [SerializeField]
    AudioClip[] Clips;
    [SerializeField]
    float minVol, maxVol, minPitch, maxPitch;
    [SerializeField]
    bool PlayOnStart;
    [SerializeField]
    public bool PlayOnCollision;

    void Start()
    {
        Source = GetComponent<AudioSource>();
        if (PlayOnStart)
            Play();
    }
    void OnCollisionEnter(Collision col)
    {
        if (PlayOnCollision)
            Play();
    }
    void OnTriggerEnter(Collider col)
    {
        if (PlayOnCollision)
            Play();
    }

    public void Play()
    {
        if (Source == null)
            return;

        Source.Stop();

        Source.clip = Clips[Random.Range(0, Clips.Length)];
        Source.volume = Random.Range(minVol, maxVol);
        Source.pitch = Random.Range(minPitch, maxPitch);

        Source.Play();
    }

}
