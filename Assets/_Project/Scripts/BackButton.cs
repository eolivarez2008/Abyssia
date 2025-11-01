using UnityEngine;
using UnityEngine.SceneManagement;

public class UniversalBackButton : MonoBehaviour
{
    public void GoToMenu()
    {
        // Charge la scène MenuPrincipal
        SceneManager.LoadScene("Menu"); // ⚠️ Mets ici le nom exact de ta scène MenuPrincipal
    }
}
