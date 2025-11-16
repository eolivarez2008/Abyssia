using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Gère le menu des paramètres (audio, résolution, etc.)
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Audio Mixer principal")]
    public AudioMixer audioMixer;
    
    [Tooltip("Slider pour le volume de la musique")]
    public Slider musicSlider;
    
    [Tooltip("Slider pour le volume des sons")]
    public Slider soundSlider;

    [Header("Graphics")]
    [Tooltip("Dropdown pour la résolution")]
    public Dropdown resolutionDropdown;

    [Header("Commands")]
    [Tooltip("Image affichant les commandes")]
    public GameObject commandsImage;

    private Resolution[] resolutions;

    public void Start()
    {
        // Initialise les sliders audio avec les valeurs actuelles
        if (audioMixer != null && musicSlider != null)
        {
            audioMixer.GetFloat("Music", out float musicValue);
            musicSlider.value = musicValue;
        }

        if (audioMixer != null && soundSlider != null)
        {
            audioMixer.GetFloat("Sound", out float soundValue);
            soundSlider.value = soundValue;
        }

        // Configure le dropdown de résolution
        SetupResolutionDropdown();

        // Active le plein écran par défaut
        Screen.fullScreen = true;
    }

    /// <summary>
    /// Configure le dropdown des résolutions disponibles
    /// </summary>
    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogWarning("SettingsMenu: resolutionDropdown non assigné!");
            return;
        }

        // Récupère toutes les résolutions uniques
        resolutions = Screen.resolutions
            .Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
            .Distinct()
            .ToArray();
        
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            // Trouve la résolution actuelle
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Définit le volume de la musique
    /// </summary>
    public void SetVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Music", volume);
        }
    }

    /// <summary>
    /// Définit le volume des effets sonores
    /// </summary>
    public void SetSoundVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Sound", volume);
        }
    }

    /// <summary>
    /// Active/désactive le plein écran
    /// </summary>
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    /// <summary>
    /// Change la résolution
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        if (resolutions == null || resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning($"SettingsMenu: Index de résolution invalide {resolutionIndex}");
            return;
        }

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    /// <summary>
    /// Affiche l'image des commandes
    /// </summary>
    public void ShowCommands()
    {
        if (commandsImage != null)
            commandsImage.SetActive(true);
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }

    /// <summary>
    /// Cache l'image des commandes
    /// </summary>
    public void HideCommands()
    {
        if (commandsImage != null)
            commandsImage.SetActive(false);
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayButtonClick();
    }
}