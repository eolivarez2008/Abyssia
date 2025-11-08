using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Musique de fond")]
    public AudioClip musiqueDeFond;
    public AudioMixerGroup musicMixer;

    [Header("Effets sonores (SFX)")]
    public AudioMixerGroup sfxMixer;

    private AudioSource musicSource;

    public static AudioManager instance;

    private void Awake()
    {
        // Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicMixer;
    }

    private void Start()
    {
        if (musiqueDeFond != null)
        {
            musicSource.clip = musiqueDeFond;
            musicSource.Play();
        }
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        if (clip == null)
        {
            return null;
        }

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = sfxMixer;
        source.Play();

        Destroy(tempGO, clip.length);
        return source;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, 1f);
    }
}
