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

        // Création automatique de la source musique
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
        else
        {
            Debug.LogWarning("AudioManager: aucune musique de fond assignée.");
        }
    }

    // Joue un son 3D ou ponctuel (effet sonore)
    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: clip audio nul.");
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

    // Optionnel : jouer un son d'UI global sans position
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, 1f);
    }
}
