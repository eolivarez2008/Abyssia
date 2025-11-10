using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Musique de fond")]
    public AudioClip musiqueDeFond;
    public AudioMixerGroup musicMixer;

    [Header("Effets sonores (SFX)")]
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
    public float uiVolume = 1f;
    [Range(0f, 1f)]
    public float gameVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    public static AudioManager instance;

    private void Awake()
    {
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

    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound, uiVolume);
    }

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

    public void PlayValidate()
    {
        PlaySound(validateSound, uiVolume);
    }

    public void PlayUseHealthPop()
    {
        PlaySound(useHealthPopSound, gameVolume);
    } 

    public void PlayReward()
    {
        PlaySound(rewardSound, uiVolume);
    }

    public void PlayPlayerNotAttack()
    {
        PlaySound(playerNotAttackSound, gameVolume);
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

    public void PlaySFX(AudioClip clip)
    {
        PlaySound(clip);
    }

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