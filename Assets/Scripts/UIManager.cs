using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Text References")]
    public TMP_Text goldText;
    public TMP_Text livesText;
    public TMP_Text waveText;

    [Header("Controls")]
    public Button sendWaveButton;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private void OnEnable()
    {
        GameManager.OnGoldChanged += UpdateGoldText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnGameOver += ShowGameOver;
        GameManager.OnWin += ShowWin;

        WaveManager.OnWaveStarted += UpdateWaveText;
        WaveManager.OnWaveComplete += OnWaveComplete;
    }

    private void OnDisable()
    {
        GameManager.OnGoldChanged -= UpdateGoldText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnGameOver -= ShowGameOver;
        GameManager.OnWin -= ShowWin;

        WaveManager.OnWaveStarted -= UpdateWaveText;
        WaveManager.OnWaveComplete -= OnWaveComplete;
    }

    private void Start()
    {
        // Initialize displays
        UpdateGoldText(GameManager.Instance.gold);
        UpdateLivesText(GameManager.Instance.lives);
        
        int totalWaves = WaveManager.Instance.waves.Length;
        waveText.text = "Wave 0 / " + totalWaves;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        
        sendWaveButton.interactable = true;
        sendWaveButton.onClick.AddListener(WaveManager.Instance.StartNextWave);
    }

    public void UpdateGoldText(int gold)
    {
        goldText.text = gold + " Credits";
    }

    public void UpdateLivesText(int lives)
    {
        livesText.text = "Lives: " + lives;
        if (lives <= 5)
        {
            livesText.color = Color.red;
        }
        else
        {
            livesText.color = Color.white;
        }
    }

    public void UpdateWaveText(int wave)
    {
        int totalWaves = WaveManager.Instance.waves.Length;
        waveText.text = "Wave " + wave + " / " + totalWaves;
        sendWaveButton.interactable = false;
    }

    public void OnWaveComplete()
    {
        sendWaveButton.interactable = true;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
    }
}
