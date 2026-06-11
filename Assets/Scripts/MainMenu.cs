using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the gameplay scene to load (must be in Build Settings).")]
    public string gameSceneName = "LevelOne";

    [Header("UI References")]
    public TMP_Text totalTimePlayedText;
    public Slider musicVolumeSlider;
    public TMP_Text creditsText;

    [Header("Team Credits")]
    [TextArea]
    public string teamMembers = "Dylan Vo and Brielle Moran";

    private float totalTimePlayed;

    void Start()
    {
       
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
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        SaveTime();
        Debug.Log("[MainMenu] Quitting game.");
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
