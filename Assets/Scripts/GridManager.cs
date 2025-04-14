using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject backgroundTilePrefab;
    public GameObject letterTilePrefab;
    public Transform backgroundParent;
    public Transform letterParent;
    public int gridSize = 5;

    private GameObject[,] letterTiles;

    void Start()
    {
        letterTiles = new GameObject[gridSize, gridSize];

        GenerateBackgroundGrid();
        SpawnRandomLetter();
        SpawnRandomLetter();
    }

    void GenerateBackgroundGrid()
    {
        GridLayoutGroup layout = backgroundParent.GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = gridSize;

        for(int y = 0; y < gridSize; y++)
        {
            for(int x = 0; x < gridSize; x++)
            {
                _ = Instantiate(backgroundTilePrefab, backgroundParent);
            }
        }
    }
    
    void SpawnRandomLetter()
    {
        List<Vector2Int> emptyCells = new();

        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
                if (letterTiles[x, y] == null)
                    emptyCells.Add(new Vector2Int(x, y));

        if (emptyCells.Count == 0) return;

        Vector2Int spawnPos = emptyCells[Random.Range(0, emptyCells.Count)];

        GameObject newTile = Instantiate(letterTilePrefab, letterParent);

        //Set letter
        newTile.GetComponent<Tile>().SetLetter('A');
        letterTiles[spawnPos.x, spawnPos.y] = newTile;
    }
}
