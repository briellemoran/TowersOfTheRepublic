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

    private float totalTimePlayed;

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

        // disable any other audio listeners in other loaded scenes
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
            // Keep the slider wired even if not set up in the Inspector.
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        totalTimePlayed = PlayerPrefs.GetFloat("TotalTimePlayed", 0f);
        UpdateTimeDisplay();

        if (creditsText != null)
        {
            creditsText.text = teamMembers;
        }
    }

    void Update()
    {
        // Accumulate time spent this session and keep the display current.
        totalTimePlayed += Time.deltaTime;
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (totalTimePlayedText != null)
        {
            float minutes = totalTimePlayed / 60f;
            totalTimePlayedText.text = "Total Time Played: " + minutes.ToString("F1") + " minutes";
        }
    }

    private void SaveTime()
    {
        PlayerPrefs.SetFloat("TotalTimePlayed", totalTimePlayed);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SaveTime();
        Time.timeScale = 1f;

        string lastLevel = PlayerPrefs.GetString("LastLevel", "");
    
        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsInactive.Include);
        foreach (AudioListener listener in listeners)
        {
            listener.enabled = true;
        }

        if (lastLevel != "" && SceneManager.sceneCount > 1)
        {
            EventSystem.current.enabled = true;
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
        SaveTime();
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

    // Make sure time is saved when the menu is disabled or the app closes.
    void OnDisable()
    {
        SaveTime();
    }

    void OnApplicationQuit()
    {
        SaveTime();
    }
}
