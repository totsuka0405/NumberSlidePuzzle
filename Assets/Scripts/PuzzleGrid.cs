using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGrid : MonoBehaviour
{
    [SerializeField] int gridSize = 3; // グリッドのサイズ（3x3, 4x4など）
    [SerializeField] float maxGridSize = 400f;
    [SerializeField] float space = 10f; // セル間のスペース
    [SerializeField] GameObject cellPrefab; // ボタンのプレハブ（ボタンの見た目のため）
    [SerializeField] Transform gameArea; // セルを配置する親オブジェクト

    private Button[,] buttons; // ボタンの2次元配列
    private Text[,] buttonTexts; // テキストの2次元配列
    public bool isClear = false; // ゲームクリア状態
    private int emptyIndex; // 空きマスのインデックス
    private InputHandler inputHandler; // InputHandlerの参照
    private List<int> numbers; // 配列の数値
    private List<GameObject> cellInstances; // セルインスタンスのリスト

    [SerializeField] GameManager gameManager;
    [SerializeField] SoundManager soundManager;


    private void Start()
    {
        
    }

    public void InitializeStage(int gameLevel)
    {
        if (inputHandler == null)
        {
            inputHandler = GetComponent<InputHandler>();
            inputHandler.OnEmptyTextUpdated += HandleEmptyTextUpdated; // イベントにリスナーを登録
        }

        if (soundManager == null)
        {
            soundManager = gameObject.GetComponent<SoundManager>();
        }

        gridSize += gameLevel;

        // セルインスタンスを再利用するためにボタンを初期化
        InitializeButtons();
        InitializePuzzle();
    }

    void InitializeButtons()
    {
        // ボタン配列を初期化
        buttons = new Button[gridSize, gridSize];
        buttonTexts = new Text[gridSize, gridSize];

        // セルインスタンスを作成するためのリストを初期化
        if (cellInstances == null)
        {
            cellInstances = new List<GameObject>();
        }

        // セルサイズの計算
        float cellSize = (maxGridSize - (space * (gridSize - 1))) / gridSize;
        float totalWidth = gridSize * cellSize + (gridSize - 1) * space;
        float totalHeight = gridSize * cellSize + (gridSize - 1) * space;
        float startX = -totalWidth / 2 + cellSize / 2;
        float startY = totalHeight / 2 - cellSize / 2;

        // ボタンを生成
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject buttonObj;

                // 既存のセルを再利用または新たに生成
                if (cellInstances.Count > i * gridSize + j)
                {
                    buttonObj = cellInstances[i * gridSize + j];
                }
                else
                {
                    buttonObj = Instantiate(cellPrefab, gameArea);
                    cellInstances.Add(buttonObj); // 新しく生成したセルをリストに追加
                }

                buttons[i, j] = buttonObj.GetComponent<Button>();
                buttonTexts[i, j] = buttonObj.GetComponentInChildren<Text>();
                int index = i * gridSize + j;

                // 既存のリスナーを削除してから追加
                buttons[i, j].onClick.RemoveAllListeners(); // これを追加
                buttons[i, j].onClick.AddListener(() => OnButtonClicked(index));

                // ボタンの位置を計算して設定
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                float xPos = startX + j * (cellSize + space);
                float yPos = startY - i * (cellSize + space);

                rectTransform.anchoredPosition = new Vector2(xPos, yPos);
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize); // セルのサイズを設定
            }
        }

        // 不要なセルを削除
        for (int i = gridSize * gridSize; i < cellInstances.Count; i++)
        {
            Destroy(cellInstances[i]); // 不要なセルを破棄
        }

        // リストを必要なサイズに縮小
        cellInstances.RemoveRange(gridSize * gridSize, cellInstances.Count - (gridSize * gridSize));
    }


    void InitializePuzzle()
    {
        isClear = false;
        numbers = new List<int>();

        for (int i = 1; i < gridSize * gridSize; i++)
        {
            numbers.Add(i);
        }
        numbers.Add(0); // 空きマスを追加

        // 解ける配置を作成するまで繰り返す
        do
        {
            Shuffle(numbers);
        } while (!IsSolvable(numbers));

        // ボタンに数値を設定
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int value = numbers[i * gridSize + j];
                buttonTexts[i, j].text = (value == 0) ? "" : value.ToString();
                if (value == 0)
                {
                    emptyIndex = i * gridSize + j;
                }
            }
        }

        // InputHandlerを初期化
        // 2次元配列を1次元配列に変換
        Button[] flatButtons = new Button[gridSize * gridSize];
        Text[] flatTexts = new Text[gridSize * gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                flatButtons[i * gridSize + j] = buttons[i, j];
                flatTexts[i * gridSize + j] = buttonTexts[i, j];
            }
        }

        inputHandler.Initialize(flatButtons, flatTexts, emptyIndex, this, gridSize); // gridSizeを渡す
    }

    bool IsSolvable(List<int> numbers)
    {
        int inversions = 0;
        int gridSize = (int)Mathf.Sqrt(numbers.Count); // グリッドのサイズを取得

        // 逆転数を計算
        for (int i = 0; i < numbers.Count; i++)
        {
            if (numbers[i] == 0) continue; // 空きマスはスキップ
            for (int j = i + 1; j < numbers.Count; j++)
            {
                if (numbers[j] == 0) continue; // 空きマスはスキップ
                if (numbers[i] > numbers[j])
                {
                    inversions++;
                }
            }
        }

        // 空きマスの行位置を計算
        int emptyRowFromBottom = gridSize - (numbers.IndexOf(0) / gridSize);

        if (gridSize % 2 != 0)
        {
            // 奇数サイズ: 逆転数が偶数であれば解ける
            return inversions % 2 == 0;
        }
        else
        {
            // 偶数サイズ: 空きマスの行位置も考慮
            if (emptyRowFromBottom % 2 == 0)
            {
                return inversions % 2 != 0;
            }
            else
            {
                return inversions % 2 == 0;
            }
        }
    }

    public void UpdateEmptyIndex(int newEmptyIndex)
    {
        emptyIndex = newEmptyIndex;
        inputHandler.UpdateEmptyIndex(newEmptyIndex);
    }

    public void CheckForWin()
    {
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            if (i < gridSize * gridSize - 1 && buttonTexts[i / gridSize, i % gridSize].text != (i + 1).ToString())
            {
                return; // まだ揃っていない
            }
        }
        isClear = true;
        Debug.Log("Congratulations! You solved the puzzle!");
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    // ボタンがクリックされたときの処理
    private void OnButtonClicked(int index)
    {
        
    }

    private void HandleEmptyTextUpdated(int newIndex)
    {
        Debug.Log("空きマスのインデックスが更新されました: " + newIndex);
        // ここで必要な処理を行います

        gameManager.IncrementMoveCount(); // 手数をカウント
                                          // 数字をスライドさせる処理
        soundManager.PlaySlideSE();
    }

    void OnDestroy()
    {
        inputHandler.OnEmptyTextUpdated -= HandleEmptyTextUpdated; // リスナーを解除
    }
}
