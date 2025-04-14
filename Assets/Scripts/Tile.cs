using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    private char currentLetter = ' ';
    private bool isEmpty = true;

    public void SetLetter(char letter)
    {
        currentLetter = letter;
        letterText.text = letter.ToString();
        isEmpty = false;
    }

    public void ClearTile()
    {
        currentLetter = ' ';
        letterText.text = "";
        isEmpty = true;
    }

    public bool IsEmpty() => isEmpty;
    public char GetLetter() => currentLetter;
}
