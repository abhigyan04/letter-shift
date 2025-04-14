using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class TargetWordManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _targetWordText;
    [SerializeField] private TextMeshProUGUI _collectedLettersText;
    [SerializeField] private WordFetcher _wordFetcher;

    public string _targetWord;
    private readonly HashSet<char> _collectedLetters = new();

    public string TargetWord => _targetWord;
    public IReadOnlyCollection<char> CollectedLetters => _collectedLetters;

    public static TargetWordManager Instance { get; private set; }

    IEnumerator WaitForWord()
    {
        while (!_wordFetcher.IsReady)
            yield return null;

        string word = _wordFetcher.TargetWord;
        SetTargetWord(word);
        UpdateWordDisplay();
        //_targetWordText.text = _targetWord;
    }

    void SetTargetWord(string word)
    {
        _targetWord = word;
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    void Start()
    {
        StartCoroutine(WaitForWord());
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
                GameManager.Instance.ForceWin();
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
