using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetWordManager : MonoBehaviour
{
    [SerializeField] private List<string> _wordList; // Fill this in the Inspector with 4-letter words
    [SerializeField] private TextMeshProUGUI _targetWordText;
    [SerializeField] private TextMeshProUGUI _collectedLettersText;

    public string _targetWord;
    private HashSet<char> _collectedLetters = new();

    public string TargetWord => _targetWord;
    public IReadOnlyCollection<char> CollectedLetters => _collectedLetters;

    public static TargetWordManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        GenerateDailyWord();
        UpdateWordDisplay();
    }

    void GenerateDailyWord()
    {
        if (_wordList.Count == 0)
        {
            Debug.LogWarning("Word list is empty!");
            _targetWord = "NULL";
            return;
        }

        DateTime start = new(2022, 1, 1);
        int dayOffset = (int)(DateTime.Today - start).TotalDays;
        _targetWord = _wordList[dayOffset % _wordList.Count].ToUpper();
    }

    public bool TryCollectLetter(char letter)
    {
        letter = char.ToUpper(letter);

        if (_targetWord.Contains(letter) && !_collectedLetters.Contains(letter))
        {
            _collectedLetters.Add(letter);
            UpdateWordDisplay();

            if (IsWordComplete())
            {
                Debug.Log("Player completed the word!");
                // Notify GameManager or trigger win condition here
            }

            return true;
        }

        return false;
    }

    public bool IsWordComplete()
    {
        foreach (char c in _targetWord)
        {
            if (!_collectedLetters.Contains(c))
                return false;
        }

        return true;
    }

    void UpdateWordDisplay()
    {
        _targetWordText.text = _targetWord;

        string display = "";
        foreach (char c in _targetWord)
        {
            display += _collectedLetters.Contains(c) ? $"{c}" : "_";
        }

        _collectedLettersText.text = display.Trim();
    }
}
