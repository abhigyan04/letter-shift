using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LetterSpawner : MonoBehaviour
{
    [SerializeField] private string _targetWord;

    [SerializeField] private List<Block> _activeBlocks = new();

    public void SetTargetWord(string word)
    {
        _targetWord = word.ToUpper();
    }

    public void UpdateBlockList(List<Block> currentBlocks)
    {
        _activeBlocks = currentBlocks;
    }

    public char GetLetterToSpawn()
    {
        // Always start with A and B
        char baseMax = 'B';

        if (_activeBlocks.Count > 0)
        {
            // Get highest letter on the board
            char highestLetterOnBoard = _activeBlocks.Max(b => b.Letter);

            // Unlock next letter beyond highest on board, capped at Z
            baseMax = (char)Mathf.Min('Z', highestLetterOnBoard + 1);
        }

        // Build spawnable list from 'A' to baseMax
        List<char> spawnableLetters = new();
        for (char c = 'A'; c <= baseMax; c++)
        {
            // Don't include letters that are in the target word
            if (!_targetWord.Contains(c))
            {
                spawnableLetters.Add(c);
            }
        }

        // If somehow all allowed letters are filtered out (e.g. target is ABC),
        // fall back to just 'A'
        if (spawnableLetters.Count == 0)
        {
            spawnableLetters.Add('A');
        }

        return spawnableLetters[Random.Range(0, spawnableLetters.Count)];
    }
}
