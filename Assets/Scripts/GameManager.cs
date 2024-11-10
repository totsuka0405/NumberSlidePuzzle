using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private PuzzleGrid puzzleGrid;
    [SerializeField] UIManager uiManager;
    int gameLevel = 0;
    int maxLevel = 2;
    bool isGamePlay;
    private float elapsedTime; // 経過時間
    private int moveCount; // 手数

    // リザルトを保存するためのリスト
    private List<(int moves, float time)> levelResults = new List<(int, float)>();

    void Start()
    {
        isGamePlay = false;
        puzzleGrid = GetComponent<PuzzleGrid>();
        if (puzzleGrid == null)
        {
            Debug.LogError("PuzzleGrid component is missing on GameManager.");
            return; // エラー処理
        }

        if (uiManager == null)
        {
            Debug.LogError("IGameUI implementation is missing on GameManager.");
            return; // エラー処理
        }
        uiManager.StartUIShow();
    }

    private void Update()
    {
        CheckGameStatus();

        if (isGamePlay)
        {
            elapsedTime += Time.deltaTime;
            uiManager.UpdateTimeDisplay(elapsedTime); // UIに経過時間を更新
        }
    }

    public void InitializeGame()
    {
        puzzleGrid.InitializeStage(gameLevel);
        uiManager.HideWinMessage();
    }

    public void CheckGameStatus()
    {
        if (puzzleGrid.isClear == true && isGamePlay == true)
        {
            uiManager.ShowWinMessage();
            isGamePlay = false;
            SaveLevelResult(); // リザルトを保存
        }
    }

    public void IncrementMoveCount()
    {
        moveCount++;
        uiManager.UpdateMoveCountDisplay(moveCount); // UIに手数を更新
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void StartGame()
    {
        uiManager.StartUIHide();
        elapsedTime = 0f; // 経過時間の初期化
        moveCount = 0; // 手数の初期化
        uiManager.UpdateMoveCountDisplay(moveCount); // UIに手数を更新
        InitializeGame();
        isGamePlay = true;
    }

    public void NextGame()
    {
        if (gameLevel == maxLevel)
        {
            Debug.Log("ゲーム終了");
            EndGame();
        }
        else
        {
            gameLevel += 1;
            StartGame();
        }
    }

    public void EndGame()
    {
        uiManager.EndUIShow();
        DisplayResults(); // リザルトを表示
        uiManager.DisplayLevelResults(levelResults); // レベル毎のリザルト表示
    }

    private void SaveLevelResult()
    {
        // 手数と時間をリストに追加
        levelResults.Add((moveCount, elapsedTime));
        Debug.Log($"Level {gameLevel} - Moves: {moveCount}, Time: {elapsedTime}");
    }

    private void DisplayResults()
    {
        // 各レベルのリザルトを表示
        for (int i = 0; i < levelResults.Count; i++)
        {
            Debug.Log($"Level {i} - Moves: {levelResults[i].moves}, Time: {levelResults[i].time}");
        }
    }
}
