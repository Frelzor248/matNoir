using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneNavigation : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}