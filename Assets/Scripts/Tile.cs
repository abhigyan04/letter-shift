using UnityEngine;
using TMPro;


public class Tile : MonoBehaviour
{
    public TextMeshProUGUI letterText;

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
    }

    public char GetLetter()
    {
        return letterText.text[0];
    }
}
