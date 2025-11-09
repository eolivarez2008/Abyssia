using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Musique de fond")]
    public AudioClip musiqueDeFond;
    public AudioMixerGroup musicMixer;

    [Header("Effets sonores (SFX)")]
    public AudioMixerGroup sfxMixer;

    [Header("Sons UI")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip menuOpenSound;
    public AudioClip menuCloseSound;

    [Header("Sons de jeu")]
    public AudioClip pickupCoinSound;
    public AudioClip pickupItemSound;
    public AudioClip chestOpenSound;
    public AudioClip playerAttackSound;
    public AudioClip playerHitSound;
    public AudioClip playerDeathSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;

    [Header("Paramètres audio")]
    [Range(0f, 1f)]
    public float uiVolume = 1f;
    [Range(0f, 1f)]
    public float gameVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

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

        // Source pour la musique
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicMixer;

        // Source pour les SFX (réutilisable)
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.outputAudioMixerGroup = sfxMixer;
    }

    private void Start()
    {
        if (musiqueDeFond != null)
        {
            musicSource.clip = musiqueDeFond;
            musicSource.Play();
        }
    }

    // Méthode générique pour jouer un son
    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // Sons UI
    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound, uiVolume);
    }

    public void PlayButtonHover()
    {
        PlaySound(buttonHoverSound, uiVolume * 0.5f);
    }

    public void PlayMenuOpen()
    {
        PlaySound(menuOpenSound, uiVolume);
    }

    public void PlayMenuClose()
    {
        PlaySound(menuCloseSound, uiVolume);
    }

    // Sons de jeu
    public void PlayPickupCoin()
    {
        PlaySound(pickupCoinSound, gameVolume);
    }

    public void PlayPickupItem()
    {
        PlaySound(pickupItemSound, gameVolume);
    }

    public void PlayChestOpen()
    {
        PlaySound(chestOpenSound, gameVolume);
    }

    public void PlayPlayerAttack()
    {
        PlaySound(playerAttackSound, gameVolume);
    }

    public void PlayPlayerHit()
    {
        PlaySound(playerHitSound, gameVolume);
    }

    public void PlayPlayerDeath()
    {
        PlaySound(playerDeathSound, gameVolume);
    }

    public void PlayEnemyHit()
    {
        PlaySound(enemyHitSound, gameVolume * 0.8f);
    }

    public void PlayEnemyDeath()
    {
        PlaySound(enemyDeathSound, gameVolume);
    }

    // Méthode pour jouer un son à une position (3D)
    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos, float volumeScale = 1f)
    {
        if (clip == null) return null;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = sfxMixer;
        source.volume = volumeScale;
        source.Play();

        Destroy(tempGO, clip.length);
        return source;
    }

    // Ancienne méthode maintenue pour compatibilité
    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip);
    }

    // Contrôle de la musique
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}