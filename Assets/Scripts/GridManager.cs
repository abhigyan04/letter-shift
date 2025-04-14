using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject gridPrefab;
    public Transform gridParent;
    public Transform backgroundParent;
    public int gridSize = 5;
    private Tile[,] grid;
    private Tile[,] background;

    void Start()
    {
        grid = new Tile[gridSize, gridSize];
        background = new Tile[gridSize, gridSize];
        GenerateGrid();
        SpawnRandomLetter();
        SpawnRandomLetter();
    }

    void GenerateGrid()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject bgObj = Instantiate(gridPrefab, backgroundParent);
                Tile bg = bgObj.GetComponent<Tile>();
                bg.ClearTile();
                background[x, y] = bg;
            }
        }
    }

    void SpawnRandomLetter()
    {
        List<Vector2Int> emptyTiles = new();

        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
                if (grid[x, y] == null)
                    emptyTiles.Add(new Vector2Int(x, y));

        if (emptyTiles.Count == 0) return;

        Vector2Int pos = emptyTiles[Random.Range(0, emptyTiles.Count)];
        GameObject tileObj = Instantiate(tilePrefab, gridParent);
        Tile tile = tileObj.GetComponent<Tile>();
        grid[pos.x, pos.y] = tile;
        tile.SetLetter('A'); // Could add weighted random later
        grid[pos.x, pos.y].GetComponent<RectTransform>().anchorMin = background[pos.x, pos.y].GetComponent<RectTransform>().anchorMin;
        grid[pos.x, pos.y].GetComponent<RectTransform>().anchorMax = background[pos.x, pos.y].GetComponent<RectTransform>().anchorMax;
        grid[pos.x, pos.y].GetComponent<RectTransform>().pivot = background[pos.x, pos.y].GetComponent<RectTransform>().pivot;
        grid[pos.x, pos.y].GetComponent<RectTransform>().localScale = background[pos.x, pos.y].GetComponent<RectTransform>().localScale;
        grid[pos.x, pos.y].GetComponent<RectTransform>().rotation = background[pos.x, pos.y].GetComponent<RectTransform>().rotation;
    }
}
