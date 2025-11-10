using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Gestionnaire centralisé pour tous les sons du jeu (musique et effets sonores)
/// Pattern Singleton avec DontDestroyOnLoad
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Musique de fond")]
    [Tooltip("Clip audio de la musique principale")]
    public AudioClip musiqueDeFond;
    [Tooltip("Groupe du mixer audio pour la musique")]
    public AudioMixerGroup musicMixer;

    [Header("Effets sonores (SFX)")]
    [Tooltip("Groupe du mixer audio pour les effets sonores")]
    public AudioMixerGroup sfxMixer;

    [Header("Liste des clips audio")]
    public AudioClip buttonClickSound;
    public AudioClip pickupCoinSound;
    public AudioClip pickupItemSound;
    public AudioClip playerAttackSound;
    public AudioClip playerNotAttackSound;
    public AudioClip playerHitSound;
    public AudioClip playerDeathSound;
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip enemyAttackSound;
    public AudioClip useHealthPotionSound;
    public AudioClip useShieldPotionSound;
    public AudioClip useSpeedPotionSound;
    public AudioClip useForcePotionSound;
    public AudioClip useHealthPopSound;
    public AudioClip validateSound;
    public AudioClip chestOpenSound;
    public AudioClip rewardSound;
    public AudioClip openDialogueSound;
    public AudioClip closeDialogueSound;
    public AudioClip nextDialogueSound;

    [Header("Paramètres audio")]
    [Range(0f, 1f)]
    [Tooltip("Volume des sons d'interface (0-1)")]
    public float uiVolume = 1f;
    [Range(0f, 1f)]
    [Tooltip("Volume des sons de gameplay (0-1)")]
    public float gameVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public static AudioManager instance;

    private void Awake()
    {
        // Pattern Singleton avec DontDestroyOnLoad
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Configuration de l'AudioSource pour la musique
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicMixer;

        // Configuration de l'AudioSource pour les effets sonores
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.outputAudioMixerGroup = sfxMixer;
    }

    private void Start()
    {
        // Lance la musique de fond si définie
        if (musiqueDeFond != null)
        {
            musicSource.clip = musiqueDeFond;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Joue un effet sonore avec un volume personnalisé
    /// </summary>
    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    // Sons d'interface
    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound, uiVolume);
    }

    public void PlayValidate()
    {
        PlaySound(validateSound, uiVolume);
    }

    public void PlayOpenDialogue()
    {
        PlaySound(openDialogueSound, uiVolume);
    }

    public void PlayCloseDialogue()
    {
        PlaySound(closeDialogueSound, uiVolume);
    }

    public void PlayNextDialogue()
    {
        PlaySound(nextDialogueSound, uiVolume);
    }

    public void PlayError()
    {
        PlaySound(playerNotAttackSound, uiVolume);
    }

    public void PlayReward()
    {
        PlaySound(rewardSound, uiVolume);
    }

    // Sons de gameplay - Ramassage
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

    // Sons de gameplay - Joueur
    public void PlayPlayerAttack()
    {
        PlaySound(playerAttackSound, gameVolume);
    }

    public void PlayPlayerNotAttack()
    {
        PlaySound(playerNotAttackSound, gameVolume);
    }

    public void PlayPlayerHit()
    {
        PlaySound(playerHitSound, gameVolume);
    }

    public void PlayPlayerDeath()
    {
        PlaySound(playerDeathSound, gameVolume);
    }

    // Sons de gameplay - Ennemis
    public void PlayEnemyHit()
    {
        PlaySound(enemyHitSound, gameVolume);
    }

    public void PlayEnemyDeath()
    {
        PlaySound(enemyDeathSound, gameVolume);
    }

    public void PlayEnemyAttack()
    {
        PlaySound(enemyAttackSound, gameVolume);
    }

    // Sons de gameplay - Potions
    public void PlayUseHealthPotion()
    {
        PlaySound(useHealthPotionSound, gameVolume);
    }

    public void PlayUseShieldPotion()
    {
        PlaySound(useShieldPotionSound, gameVolume);
    }

    public void PlayUseSpeedPotion()
    {
        PlaySound(useSpeedPotionSound, gameVolume);
    }

    public void PlayUseForcePotion()
    {
        PlaySound(useForcePotionSound, gameVolume);
    }

    public void PlayUseHealthPop()
    {
        PlaySound(useHealthPopSound, gameVolume);
    }

    // Méthode générique pour jouer n'importe quel effet sonore
    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip);
    }

    // Contrôles de la musique
    /// <summary>
    /// Définit le volume de la musique
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    /// <summary>
    /// Met la musique en pause
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    /// <summary>
    /// Reprend la musique
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }

    /// <summary>
    /// Arrête complètement la musique
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
}