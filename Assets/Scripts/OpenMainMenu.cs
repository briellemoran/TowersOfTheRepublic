using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OpenMainMenu : MonoBehaviour
{
    // open the main menu
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale = 0f;
            PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();
            EventSystem.current.enabled = false;
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }
}
