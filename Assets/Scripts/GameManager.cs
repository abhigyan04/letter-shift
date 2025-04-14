using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    [SerializeField] private float _travelTime = 0.2f;
    [SerializeField] private LetterSpawner _letterSpawner;
    [SerializeField] private Transform _boardShakeTarget;


    [SerializeField] private GameObject _winScreen, _loseScreen;
    
    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _state;
    private int _round;

    private BlockType GetBlockTypeByLetter(char letter) => _types.First(t => t.Letter == letter);

    public static GameManager Instance { get; private set; }

    public GameState CurrentState => _state;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        ChangeState(GameState.GenerateLevel);
        _letterSpawner.SetTargetWord(TargetWordManager.Instance.TargetWord);
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                _winScreen.SetActive(true);
                break;
            case GameState.Lose:
                _loseScreen.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);
    }

    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        var center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        Camera.main.transform.position = new Vector3(center.x, center.y + 1.0f, -10);

        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        _letterSpawner.UpdateBlockList(_blocks);

        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();

        if(amount == 2)
        {
            foreach(var node in freeNodes.Take(2))
            {
                SpawnBlock(node, Random.value > 0.8 ? 'B' : 'A');
            }
        }
        else if(amount == 1)
        {
            foreach (var node in freeNodes.Take(1))
            {
                char letterToSpawn = _letterSpawner.GetLetterToSpawn();
                SpawnBlock(node, letterToSpawn);
            }
        }

        if (freeNodes.Count() == 1)
        {
            ChangeState(GameState.Lose);
            return;
        }

        ChangeState(GameState.WaitingInput);
    }

    void SpawnBlock(Node node, char letter)
    {
        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
        block.Init(GetBlockTypeByLetter(letter));
        block.SetBlock(node);
        _blocks.Add(block);

        if(TargetWordManager.Instance._targetWord.Contains(letter))
        {
            block.transform.localScale = Vector3.zero;
            block.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
    }
    
    void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);

        bool hasMoved = false;
        bool hasMerged = false;

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            var next = block.Node;
            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null)
                {
                    // We know a node is present
                    // If it's possible to merge, set merge
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.CanMerge(block.Letter))
                    {
                        block.MergeBlock(possibleNode.OccupiedBlock);
                        hasMerged = true;
                        break;
                    }
                    // Otherwise, can we move to this spot?
                    else if (possibleNode.OccupiedBlock == null)
                    {
                        next = possibleNode;
                        hasMoved = true;
                    }
                    else
                    {
                        break;
                    }
                }
            } while (next != block.Node);
        }

        var sequence = DOTween.Sequence();

        foreach (var block in orderedBlocks)
        {
            var movePoint = block.MergingBlock != null ? block.MergingBlock.Node.Pos : block.Node.Pos;

            sequence.Insert(0, block.transform.DOMove(movePoint, _travelTime).SetEase(Ease.InQuad));
        }

        sequence.OnComplete(() => {
            var mergeBlocks = orderedBlocks.Where(b => b.MergingBlock != null).ToList();
            foreach (var block in mergeBlocks)
            {
                MergeBlocks(block.MergingBlock, block);
            }
            if (hasMoved || hasMerged)
            {
                ChangeState(GameState.SpawningBlocks);
            }
            else
            {
                DoShake();
                ChangeState(GameState.WaitingInput);
            }
        });
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        char newLetter = mergingBlock.Letter < 'Z' ? (char)(mergingBlock.Letter + 1) : 'Z';

        SpawnBlock(baseBlock.Node, newLetter);

        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);

        switch(newLetter)
        {
            case 'G':
                if(_letterSpawner._maxSpawnLetter == 'B')
                    _letterSpawner.UnlockNextLetter();
                break;
            case 'M':
                if (_letterSpawner._maxSpawnLetter == 'C')
                    _letterSpawner.UnlockNextLetter();
                break;
            case 'R':
                if (_letterSpawner._maxSpawnLetter == 'D')
                    _letterSpawner.UnlockNextLetter();
                break;
            case 'U':
                if (_letterSpawner._maxSpawnLetter == 'E')
                    _letterSpawner.UnlockNextLetter();
                break;
            default:
                break;
        }
    }

    public void RemoveBlock(Block block)
    {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(n => n.Pos == pos);
    }

    void DoShake()
    {
        if (_boardShakeTarget == null) return;

        _boardShakeTarget.DOShakePosition(
            duration: 0.2f,
            strength: new Vector3(0.1f, 0.1f, 0),
            vibrato: 10,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
    }

    public void ForceWin()
    {
        if(_state != GameState.Win && _state != GameState.Lose)
        {
            ChangeState(GameState.Win);
        }
    }
}

[Serializable]
public struct BlockType
{
    public char Letter;
    public Color Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}
