using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour
{
    [Header("Shared")]
    [SerializeField] private AudioSource triggeredSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource footstepSource;
    public MovementSystem ms; // assign your player here

    [Header("1) Triggered")]
    public bool triggerPlay;
    [SerializeField] private AudioClip[] triggerClips = new AudioClip[3];
    [Range(0f, 1f)] public float triggerVolume = 1f;
    [Range(0f, 0.5f)] public float triggerPitchJitter = 0f;

    [Header("2) Ambient random")]
    [SerializeField] private AudioClip[] ambientClips;
    public float ambientMinSilence = 2f;
    public float ambientMaxSilence = 6f;
    [Range(0f, 1f)] public float ambientVolume = 1f;
    [Range(0f, 0.5f)] public float ambientPitchJitter = 0.05f;
    public bool autoStartAmbient = true;

    [Header("3) Footsteps")]
    [SerializeField] private AudioClip footstepClip; // your 8-second walking clip
    [Range(0f, 1f)] public float footstepVolume = 1f;
    [Range(0f, 0.1f)] public float footstepPitchJitter = 0.05f;
    public float footstepFadeOutSpeed = 5f; // higher = faster fade out

    private bool _armed = true;
    private Coroutine _ambientCo;

    void Awake()
    {
        // Automatically create AudioSources if missing
        triggeredSource = GetOrMakeSource("TriggeredSource", triggeredSource);
        ambientSource = GetOrMakeSource("AmbientSource", ambientSource);
        footstepSource = GetOrMakeSource("FootstepSource", footstepSource);

        triggeredSource.playOnAwake = false;
        ambientSource.playOnAwake = false;

        footstepSource.playOnAwake = false;
        footstepSource.loop = true; // loop long walking clip
        footstepSource.volume = 0f; // start muted
    }

    void Start()
    {
        if (autoStartAmbient) StartAmbient();
        if (footstepClip != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.Play();
        }
    }

    void Update()
    {
        HandleTriggeredFlag();
        HandleFootstepsFadeOut();
    }

    #region Triggered Sounds
    private void HandleTriggeredFlag()
    {
        if (triggerPlay && _armed)
        {
            var clip = PickRandom(triggerClips);
            if (clip)
            {
                triggeredSource.pitch = 1f + Random.Range(-triggerPitchJitter, triggerPitchJitter);
                triggeredSource.PlayOneShot(clip, triggerVolume);
            }
            _armed = false;
        }
        else if (!triggerPlay && !_armed)
        {
            _armed = true;
        }
    }
    #endregion

    #region Ambient
    public void StartAmbient()
    {
        if (_ambientCo == null && ambientClips != null && ambientClips.Length > 0)
            _ambientCo = StartCoroutine(AmbientLoop());
    }

    public void StopAmbient()
    {
        if (_ambientCo != null)
        {
            StopCoroutine(_ambientCo);
            _ambientCo = null;
        }
        ambientSource.Stop();
    }

    private IEnumerator AmbientLoop()
    {
        while (true)
        {
            float wait = Random.Range(Mathf.Min(ambientMinSilence, ambientMaxSilence),
                                      Mathf.Max(ambientMinSilence, ambientMaxSilence));
            yield return new WaitForSeconds(wait);

            var clip = PickRandom(ambientClips);
            if (clip)
            {
                ambientSource.volume = ambientVolume;
                ambientSource.pitch = 1f + Random.Range(-ambientPitchJitter, ambientPitchJitter);
                ambientSource.PlayOneShot(clip);
                yield return new WaitWhile(() => ambientSource.isPlaying);
            }
            else yield return null;
        }
    }
    #endregion

    #region Footsteps Fade-Out
    private void HandleFootstepsFadeOut()
    {
        if (ms == null || footstepSource == null || footstepClip == null) return;

        bool moving = ms.Velocity.magnitude > 0.1f && ms.IsGrounded;

        if (moving)
        {
            // Instant start at full volume
            footstepSource.volume = footstepVolume;

            // Randomize pitch slightly for natural feel
            footstepSource.pitch = 1f + Random.Range(-footstepPitchJitter, footstepPitchJitter);
        }
        else
        {
            // Fade out smoothly
            footstepSource.volume = Mathf.MoveTowards(footstepSource.volume, 0f, footstepFadeOutSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Helpers
    private AudioSource GetOrMakeSource(string name, AudioSource existing)
    {
        if (existing != null) return existing;

        var t = transform.Find(name);
        if (t && t.TryGetComponent<AudioSource>(out var found)) return found;

        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var src = go.AddComponent<AudioSource>();
        src.spatialBlend = 0f; // 2D sound
        src.loop = false;
        src.playOnAwake = false;
        return src;
    }

    private static AudioClip PickRandom(AudioClip[] list)
    {
        if (list == null || list.Length == 0) return null;
        return list[Random.Range(0, list.Length)];
    }
    #endregion
}
