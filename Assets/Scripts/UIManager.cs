using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] SoundManager soundManager;
    [SerializeField] GameObject winMessage;
    [SerializeField] GameObject startUI;
    [SerializeField] GameObject endUI;
    [SerializeField] GameObject mainGameUI;
    [SerializeField] Button resetButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button startButton;
    [SerializeField] Button endButton;
    [SerializeField] Text timeText;
    [SerializeField] Text moveCountText;
    [SerializeField] Text timeTextResult;
    [SerializeField] Text moveCountTextResult;

    [SerializeField] Text levelResultsText;  // 各レベルの結果を表示するテキスト
    [SerializeField] Text totalMovesText;    // 総手数を表示するテキスト
    [SerializeField] Text totalTimeText;     // 総時間を表示するテキスト
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        UpdateTimeDisplay(0);
        UpdateMoveCountDisplay(0);

        volumeSlider.value = soundManager.GetVolume();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        resetButton.onClick.AddListener(OnResetButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        startButton.onClick.AddListener(OnResetButtonClicked);
        endButton.onClick.AddListener(ObResultButtonClicked);
    }

    public void UpdateTimeDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = string.Format("時間 : {0:D2}:{1:D2}", minutes, seconds);
        timeTextResult.text = string.Format("時間 : {0:D2}:{1:D2}", minutes, seconds);
    }

    public void UpdateMoveCountDisplay(int count)
    {
        moveCountText.text = "手数 : " + count;
        moveCountTextResult.text = "手数 : " + count;
    }

    public void DisplayLevelResults(List<(int moves, float time)> results)
    {
        levelResultsText.text = "";
        int totalMoves = 0;
        float totalTime = 0f;

        for (int i = 0; i < results.Count; i++)
        {
            int minutes = Mathf.FloorToInt(results[i].time / 60);
            int seconds = Mathf.FloorToInt(results[i].time % 60);
            levelResultsText.text += $"Level {i + 1} - 手数: {results[i].moves}, 時間: {minutes:D2}:{seconds:D2}\n";
            totalMoves += results[i].moves;
            totalTime += results[i].time;
        }

        totalMovesText.text = "総手数 : " + totalMoves;
        int totalMinutes = Mathf.FloorToInt(totalTime / 60);
        int totalSeconds = Mathf.FloorToInt(totalTime % 60);
        totalTimeText.text = string.Format("総時間 : {0:D2}:{1:D2}", totalMinutes, totalSeconds);
    }

    public void StartUIHide()
    {
        startUI.SetActive(false);
        mainGameUI.SetActive(true);
    }

    public void StartUIShow()
    {
        startUI.SetActive(true);
        mainGameUI.SetActive(false);
    }

    public void EndUIShow()
    {
        endUI.SetActive(true);
        mainGameUI.SetActive(false);
    }

    public void EndUIHide()
    {
        endUI.SetActive(false);
        startUI.SetActive(true);
    }

    public void ShowWinMessage()
    {
        winMessage.SetActive(true);
    }

    public void HideWinMessage()
    {
        winMessage.SetActive(false);
    }

    private void OnResetButtonClicked()
    {
        gameManager.StartGame();
        if (startUI)
        {
            StartUIHide();
        }
        soundManager.PlaySlideSEUI();
    }

    private void OnNextButtonClicked()
    {
        gameManager.NextGame();
        soundManager.PlaySlideSEUI();
    }

    private void OnVolumeChanged(float volume)
    {
        if (soundManager != null)
        {
            soundManager.SetVolume(volume);
        }
    }

    private void ObResultButtonClicked()
    {
        EndUIHide();
        soundManager.PlaySlideSEUI();
    }
}
