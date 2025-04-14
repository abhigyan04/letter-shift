using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LetterSpawner : MonoBehaviour
{
    [SerializeField] private string _targetWord;

    [SerializeField] private List<Block> _activeBlocks = new();

    public char _maxSpawnLetter = 'B';

    public void SetTargetWord(string word)
    {
        _targetWord = word.ToUpper();
    }

    public void UpdateBlockList(List<Block> currentBlocks)
    {
        _activeBlocks = currentBlocks;
    }

    public void UnlockNextLetter()
    {
        if (_maxSpawnLetter < 'Z')
        {
            _maxSpawnLetter = (char)(_maxSpawnLetter + 1);
            Debug.Log("New spawn letter unlocked");
        }
    }

    public char GetLetterToSpawn()
    {
        List<char> spawnableLetters = new();

        for (char c = 'A'; c <= _maxSpawnLetter; c++)
        {
            if (!_targetWord.Contains(c))
            {
                spawnableLetters.Add(c);
            }
        }

        if (spawnableLetters.Count == 0)
        {
            Debug.LogWarning("No valid letters to spawn - defaulting to 'A'");
            return 'A';
        }

        return spawnableLetters[Random.Range(0, spawnableLetters.Count)];
    }
}
