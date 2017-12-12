using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
    public GameObject cellItemPrefab;                           // tile prefab
    public Transform grid;

    Transform[] lines;                                          // tile lines
    public Tile[] items;                                        // tile items
    public Vector3 cellSize = new Vector3(1f, 0.96f, 1f);       // tile size
    public Vector3 cellScale = new Vector3(1f, 1f, 1f);         // tile scale

    List<Tile> choiceList;
    Line[] lineList;
    [Range(7, 14, order = 1)]
    public int totalCols = 7;
    [Range(6, 8, order = 1)]
    public int totalRows = 6;
    int oldX = -1, oldY = -1, oldType = -1;

    //Score
    int score = 0;
    public Text scoreText;
    //Moves
    public Text movesText;
    public int moves = 10;
    //GameOver
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Button restartButton;
    bool gameOver = false;

    void Start()
    {
        if (gameOverPanel)
            gameOverPanel.SetActive(false);
        choiceList = new List<Tile>();
        RefreshScore();
        RefreshMoves();
        InitArena();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DoneDrag();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        PointerEventData pointer = eventData;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        foreach (RaycastResult rr in raycastResults)
        {
            Tile tile = rr.gameObject.GetComponent<Tile>();
            if (tile)
            {
                ChoiceCell(tile, tile.lineScript.idx, tile.idx, tile.GetTileType(), tile.isMove);
                break;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gameOver)
            return;
        PointerEventData pointer = eventData;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        foreach (RaycastResult rr in raycastResults)
        {
            Tile tile = rr.gameObject.GetComponent<Tile>();
            if (tile)
            {
                ChoiceCell(tile, tile.lineScript.idx, tile.idx, tile.GetTileType(), tile.isMove);
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DoneDrag();
    }

    Vector3 CalcX(int x)
    {
        return (x - totalCols / 2) * Vector3.right * cellSize.x + Vector3.down * (totalRows + x % 2 - 1.5f) * 0.5f * cellSize.y;
    }

    Vector3 CalcY(int y)
    {
        return Vector3.up * y * cellSize.y;
    }

    Vector3 CalcXY(int x, int y)
    {
        return CalcX(x) + CalcY(y);
    }

    // init game, draw tile grid
    void InitArena()
    {
        lines = new Transform[totalCols];
        lineList = new Line[lines.Length];

        // tile line loop
        for (int i = 0; i < lines.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(grid);
            Line script = go.AddComponent<Line>();
            script.idx = i;
            Transform tf = go.transform;
            lines[i] = tf;
            tf.SetParent(grid);
            tf.localScale = Vector3.one;
            tf.localPosition = CalcX(i);
            go.name = "Line" + (i + 1).ToString("000");
            go.layer = gameObject.layer;

            script.items = new Tile[totalRows];
            // tile loop in some line
            for (int j = 0; j < script.items.Length; j++)
            {
                GameObject g = Instantiate(cellItemPrefab) as GameObject;
                g.name = "Tile" + (j + 1).ToString("000");
                Transform t = g.transform;
                Tile c = g.GetComponent<Tile>();
                c.height = cellSize.y;
                c.gameManager = this;
                c.SetTileType(Random.Range(0, 5) % 5);
                c.lineScript = script;
                script.items[j] = c;
                c.idx = j;
                t.SetParent(tf);
                t.localPosition = CalcY(j);
                t.localScale = cellScale;
                t.localRotation = Quaternion.identity;
            }
            script.idx = i;
            lineList[i] = script;
        }
        items = GetComponentsInChildren<Tile>();
    }

    // choice tile
    public void ChoiceCell(Tile cell, int x, int y, int type, bool isMove)
    {
        if (oldX == x && oldY == y) return;
        if ((oldType != -1 && oldType != type) || choiceList.Contains(cell))
        {
            DoneDrag();
            cell.UnSetChoice();
            return;
        }
        if (isMove) return;
        choiceList.Add(cell);
        cell.isMove = true;
        cell.SetChoice();
        oldX = x;
        oldY = y;
        oldType = type;
    }

    // clear match tiles & sort tile grid when dragged
    void DoneDrag()
    {
        oldX = -1;
        oldY = -1;
        oldType = -1;
        if (choiceList.Count < 3)
        {
            foreach (Tile cb in choiceList)
            {
                cb.isMove = false;
                cb.UnSetChoice();
            }
            choiceList.Clear();
            return;
        }

        score += (int)Mathf.Pow(2, choiceList.Count);
        RefreshScore();

        choiceList.Clear();
        foreach (Transform item in lines)
            item.SendMessage("SortCells", SendMessageOptions.DontRequireReceiver);

        moves--;
        RefreshMoves();
        if (moves <= 0)
        {
            gameOver = true;
            StartCoroutine (GameOver());
        }
    }

    private void RefreshScore()
    {
        scoreText.text = score.ToString();
    }

    private void RefreshMoves()
    {
        movesText.text = moves.ToString();
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        ShowGameOverPanel();
    }

    private void ShowGameOverPanel()
    {
        if(gameOverPanel && !gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(true);
            finalScoreText.text = "Score: " + score.ToString();
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
