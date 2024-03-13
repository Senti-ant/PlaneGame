using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class WebGLVideo : MonoBehaviour
{
    [SerializeField] string VideoFileName;
    [SerializeField] bool PlayOnStart;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayOnStart)
            Play();
    }

    public void Play()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, VideoFileName);
        player.url = path;
        player.Play();
    }
}
