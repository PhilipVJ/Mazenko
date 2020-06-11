using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static List<GameObject> audioSources;

    public AudioClip jump;
    public AudioClip point;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip buttonPress;
    public AudioClip goal;
    public AudioClip mainMenuMusic;
    public AudioClip endMusic;
    private static SoundManager sManager;
    private static GameObject backgroundPlayer;
    private static int currentlyPlaying;
    private static ConcurrentQueue<TempSound> cq = new ConcurrentQueue<TempSound>();

    private void Awake()
    {
        if (sManager == null || sManager != this)
            sManager = this;

        if (backgroundPlayer == null)
        {
            MakeBackgroundMusicPlayer();
        }
    }

    void Start()
    {
        audioSources = new List<GameObject>();
        if (currentlyPlaying == 0 || currentlyPlaying != GameManager.currentLevel)
        {
            backgroundPlayer.GetComponent<AudioSource>().volume = 0f;
            StartCoroutine(FadeInMusic());
            currentlyPlaying = GameManager.currentLevel;
            SetBackgroundMusic();
        }
    }

    private IEnumerator FadeInMusic()
    {
        bool done = false;
        while (!done)
        {
            backgroundPlayer.GetComponent<AudioSource>().volume += 0.05f;
            yield return new WaitForSeconds(0.3f);
            if (backgroundPlayer.GetComponent<AudioSource>().volume >= 0.5)
            {
                done = true;
            }
        }
    }

    public void BeginFadeOutMusic()
    {
        StartCoroutine(FadeOutMusic());
    }
    private IEnumerator FadeOutMusic()
    {
        bool done = false;
        while (!done)
        {
            backgroundPlayer.GetComponent<AudioSource>().volume -= 0.05f;
            yield return new WaitForSeconds(0.3f);
            if (backgroundPlayer.GetComponent<AudioSource>().volume <= 0.0f)
            {
                done = true;
            }
        }
    }

    private void SetBackgroundMusic()
    {
        backgroundPlayer.GetComponent<AudioSource>().Stop();
        switch (GameManager.currentLevel)
        {
            case 0:
                backgroundPlayer.GetComponent<AudioSource>().clip = mainMenuMusic;
                backgroundPlayer.GetComponent<AudioSource>().Play();
                break;
            case 1:
                backgroundPlayer.GetComponent<AudioSource>().clip = level1Music;
                backgroundPlayer.GetComponent<AudioSource>().Play();
                break;
            case 2:
                backgroundPlayer.GetComponent<AudioSource>().clip = level2Music;
                backgroundPlayer.GetComponent<AudioSource>().Play();
                break;
            case 3:
                backgroundPlayer.GetComponent<AudioSource>().clip = level3Music;
                backgroundPlayer.GetComponent<AudioSource>().Play();
                break;
        }

        if ((GameManager.lastLevel + 1) == GameManager.currentLevel) // End music should play
        {
            backgroundPlayer.GetComponent<AudioSource>().clip = endMusic;
            backgroundPlayer.GetComponent<AudioSource>().Play();
        }
    }

    private void MakeBackgroundMusicPlayer()
    {
        backgroundPlayer = new GameObject();
        GameObject.DontDestroyOnLoad(backgroundPlayer);
        backgroundPlayer.transform.position = Vector3.zero;
        backgroundPlayer.AddComponent<AudioSource>();
        backgroundPlayer.GetComponent<AudioSource>().loop = true;
    }

    private void Update()
    {
        // Check if the sounds has finished playing - if yes - remove the gameobjects
        List<int> indexesToRemove = new List<int>();
        for (int i = 0; i < audioSources.Count; i++)
        {
            AudioSource source = audioSources[i].GetComponent<AudioSource>();
            if (!source.isPlaying)
            {
                indexesToRemove.Add(i);
            }
        }
        if (indexesToRemove.Count > 0)
        {
            // Remove GameObjects from array
            foreach (var index in indexesToRemove)
            {
                GameObject toRemove = audioSources[index];
                audioSources.RemoveAt(index);
                Destroy(toRemove);
            }
        }

        // Check for queued sounds to generate
        if (cq.TryDequeue(out TempSound sound))
        {
            GenerateAudio(sound.position, sound.soundName);
        }
    }

    public static SoundManager GetInstance()
    {
        return sManager;
    }

    public void QueueAudio(float3 position, string soundName)
    {
        TempSound tempSound = new TempSound { position = position, soundName = soundName };
        cq.Enqueue(tempSound);
    }


    private void GenerateAudio(float3 position, string name)
    {
        GameObject holderOfSource = new GameObject();
        // Set position of sound
        holderOfSource.transform.position = position;
        // Add to gameObject
        holderOfSource.AddComponent<AudioSource>();
        // Add to list
        audioSources.Add(holderOfSource);
        // Play sound
        switch (name)
        {
            case "jump":
                holderOfSource.GetComponent<AudioSource>().volume = 0.8f;
                holderOfSource.GetComponent<AudioSource>().PlayOneShot(jump);
                break;
            case "point":
                holderOfSource.GetComponent<AudioSource>().volume = 0.5f;
                holderOfSource.GetComponent<AudioSource>().PlayOneShot(point);
                break;
            case "goal":
                holderOfSource.GetComponent<AudioSource>().PlayOneShot(goal);
                break;
            case "buttonPress":
                holderOfSource.GetComponent<AudioSource>().PlayOneShot(buttonPress);
                break;
            default: break;
        };
    }
}
