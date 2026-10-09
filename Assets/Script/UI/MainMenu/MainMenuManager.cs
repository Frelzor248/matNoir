using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void Equipment()
    {
        SceneManager.LoadScene("Equipement");
    }

    public void Settings()
    {
        Debug.Log("Ouverture des paramètres");
    }

    public void Quit()
    {
        Debug.Log("Quitter le jeu");

        Application.Quit();
    }
}