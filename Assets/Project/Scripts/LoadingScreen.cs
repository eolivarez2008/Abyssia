using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreen;   // Canvas parent de l’écran de chargement
    public Image progressBar;          // Image type = Filled

    // Appelle cette fonction pour charger une scène avec écran de chargement
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // Active l’écran de chargement
        loadingScreen.SetActive(true);
        progressBar.fillAmount = 0f;

        // Commence à charger la scène en arrière-plan
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // La progression va de 0 à 0.9 tant que la scène n’est pas prête
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.fillAmount = progress;

            // Quand la scène est prête, termine le chargement
            if (operation.progress >= 0.9f)
            {
                // Optionnel : ajoute un petit délai pour le fondu ou l’animation
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
