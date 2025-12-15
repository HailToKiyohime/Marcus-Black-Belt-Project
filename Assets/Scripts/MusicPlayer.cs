using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    private AudioSource audioSource;

    [Header("Allowed Scenes for Music")]
    public string[] allowedScenes = { "Main Menu", "Map", "Credits", "WinningUltimate", "Winning Scene" };

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f; // seconds for fade-in/out
    [Range(0f, 1f)]
    public float maxVolume = 0.4f; // maximum volume for music

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.loop = true;
                if (!audioSource.isPlaying)
                    audioSource.Play();
                audioSource.volume = 0f; // start silent and fade in
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            // Handle the current active scene immediately
            CheckScene(SceneManager.GetActiveScene());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene(scene);
    }

    private void CheckScene(Scene scene)
    {
        if (audioSource == null) return;

        bool allowed = false;
        foreach (var s in allowedScenes)
        {
            if (scene.name == s)
            {
                allowed = true;
                break;
            }
        }

        if (allowed)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Fade in to maxVolume
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(maxVolume, fadeDuration));
        }
        else
        {
            // Fade out to 0
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeVolume(0f, fadeDuration));
        }
    }

    private IEnumerator FadeVolume(float targetVolume, float duration)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        // Stop the audio if faded out completely
        if (targetVolume == 0f && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
