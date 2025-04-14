using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using static System.Net.WebRequestMethods;

public class WordFetcher : MonoBehaviour
{
    [SerializeField] private string wordListUrl = "https://raw.githubusercontent.com/abhigyan04/letter-shift/refs/heads/main/Assets/Data/wordlist.txt";

    public string TargetWord { get; private set; }
    public bool IsReady { get; private set; } = false;

    private List<string> _words = new();

    void Start()
    {
        StartCoroutine(DownloadWordList());
    }

    IEnumerator DownloadWordList()
    {
        UnityWebRequest request = UnityWebRequest.Get(wordListUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download word list: " + request.error);
            yield break;
        }

        string[] allWords = request.downloadHandler.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Filter to 4-letter words (or 5 if you're doing Wordle-style)
        _words = allWords.Where(w => w.Length == 4).Select(w => w.ToUpper()).ToList();

        if (_words.Count == 0)
        {
            Debug.LogError("No valid words found in list.");
            yield break;
        }

        // Pick a random word
        TargetWord = _words[Random.Range(0, _words.Count)];

        //Pick daily word
        //int dayOffset = (int)(System.DateTime.Today - new System.DateTime(2022, 1, 1)).TotalDays;
        //TargetWord = _words[dayOffset % _words.Count];

        IsReady = true;

        Debug.Log("Target Word: " + TargetWord);
    }
}
