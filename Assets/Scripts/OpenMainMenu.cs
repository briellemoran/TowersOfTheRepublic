using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OpenMainMenu : MonoBehaviour
{
    public static EventSystem savedLevelEventSystem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.FlushPlaytimeNow();
            }

            Time.timeScale = 0f;
            PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();

            EventSystem currentSystem = EventSystem.current;
            if (currentSystem != null)
            {
                savedLevelEventSystem = currentSystem;
                currentSystem.enabled = false;
            }

            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }
}