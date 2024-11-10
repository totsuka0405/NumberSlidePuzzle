using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    public delegate void EmptyTextUpdatedHandler(int newIndex); // デリゲートの定義
    public event EmptyTextUpdatedHandler OnEmptyTextUpdated; // イベントの定義

    private Button[,] buttons; // ボタンの2次元配列
    private Text[,] buttonTexts; // 各ボタンのテキスト
    private int emptyIndex; // 空きマスのインデックス
    private PuzzleGrid puzzleGrid; // SlidePuzzleの参照
    private int gridSize; // グリッドサイズ

    public void Initialize(Button[] buttons, Text[] buttonTexts, int emptyIndex, PuzzleGrid puzzleGrid, int gridSize)
    {
        this.gridSize = gridSize; // gridSizeを受け取る
        this.buttons = new Button[gridSize, gridSize]; // ボタンを2次元配列に変換
        this.buttonTexts = new Text[gridSize, gridSize]; // テキストを2次元配列に変換
        this.emptyIndex = emptyIndex;
        this.puzzleGrid = puzzleGrid;

        // 1次元配列から2次元配列に変換
        int index = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                this.buttons[i, j] = buttons[index];
                this.buttonTexts[i, j] = buttonTexts[index];
                index++;
            }
        }

        // ボタンにクリックイベントを追加
        for (int i = 0; i < buttons.Length; i++)
        {
            int localIndex = i; // ローカル変数を作成
            buttons[i].onClick.AddListener(() => OnButtonClick(localIndex));
        }
    }

    public void UpdateEmptyIndex(int newEmptyIndex)
    {
        emptyIndex = newEmptyIndex;
    }

    private void OnButtonClick(int index)
    {
        // SlidePuzzleから呼び出すためのイベント
        if (IsAdjacent(index, emptyIndex))
        {
            // 空きマスのテキストを更新
            string newText = buttonTexts[index / gridSize, index % gridSize].text;
            buttonTexts[emptyIndex / gridSize, emptyIndex % gridSize].text = newText;
            buttonTexts[index / gridSize, index % gridSize].text = "";

            // 空きマスのインデックスを更新
            emptyIndex = index;

            // 空きマスのテキストが更新されたことを通知
            OnEmptyTextUpdated?.Invoke(emptyIndex);

            // SlidePuzzleにクリア判定を通知
            puzzleGrid.CheckForWin();
        }
    }

    private bool IsAdjacent(int index1, int index2)
    {
        int row1 = index1 / gridSize;
        int col1 = index1 % gridSize;
        int row2 = index2 / gridSize;
        int col2 = index2 % gridSize;

        return (Mathf.Abs(row1 - row2) + Mathf.Abs(col1 - col2) == 1);
    }
}
