using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the gameplay scene to load (must be in Build Settings).")]
    public string gameSceneName = "LevelOne";

    [Header("UI References")]
    public TMP_Text totalTimePlayedText;
    public Slider musicVolumeSlider;
    public TMP_Text creditsText;
    public Button startButton;
    public TMP_Text startButtonText;

    [Header("Team Credits")]
    [TextArea]
    public string teamMembers = "Dylan Vo and Brielle Moran";

    void Start()
    {
        if (SceneManager.sceneCount == 1)
        {
            PlayerPrefs.DeleteKey("LastLevel");
        }

        string lastLevel = PlayerPrefs.GetString("LastLevel", "");

        if (lastLevel != "" && startButtonText != null)
        {
            startButtonText.text = "CONTINUE";
        }
        else if (startButtonText != null)
        {
            startButtonText.text = "START GAME";
        }

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include);
        foreach (AudioListener listener in listeners)
        {
            if (listener.gameObject.scene.name != "MainMenu")
                listener.enabled = false;
        }

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        AudioListener.volume = savedVolume;
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = savedVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        UpdateTimeDisplay();

        if (creditsText != null)
        {
            creditsText.text = teamMembers;
        }
    }

    private void UpdateTimeDisplay()
    {
        if (totalTimePlayedText != null)
        {
            float totalSeconds = PlayerPrefs.GetFloat("TotalTimePlayed", 0f);
            float minutes = totalSeconds / 60f;
            totalTimePlayedText.text = "Total Time Played: " + minutes.ToString("F1") + " minutes";
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        string lastLevel = PlayerPrefs.GetString("LastLevel", "");

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include);
        foreach (AudioListener listener in listeners)
        {
            listener.enabled = true;
        }

        if (lastLevel != "" && SceneManager.sceneCount > 1)
        {
            if (OpenMainMenu.savedLevelEventSystem != null)
            {
                OpenMainMenu.savedLevelEventSystem.enabled = true;
            }

            PlayerPrefs.DeleteKey("LastLevel");
            SceneManager.UnloadSceneAsync("MainMenu");
        }
        else
        {
            PlayerPrefs.DeleteKey("LastLevel");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteKey("LastLevel");
        PlayerPrefs.Save();
        Debug.Log("[MainMenu] Quitting game.");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Application.Quit();
    }

    public void SetMusicVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}