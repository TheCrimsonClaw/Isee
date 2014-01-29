using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour
{
    public AudioClip[] musicFiles;
    public float fadeTime = 3.0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (GameObject.FindWithTag("MusicPlayer") == null)
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!audio.isPlaying)
        {
            audio.clip = musicFiles[Random.Range(0, musicFiles.Length)];
            audio.Play();
        }

        if (audio.time < fadeTime)
        {
            audio.volume = audio.time / fadeTime;
        }
        else if (audio.clip.length - audio.time < fadeTime)
        {
            audio.volume = audio.clip.length - audio.time / fadeTime;
        }
    }
}