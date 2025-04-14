using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _howToPlayPanel;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private GameObject _gameAssets;
    [SerializeField] private GameObject _inGameHowToPlayText;
    private bool _inGameHowToPlayTextVisible = false;

    public void PlayGame()
    {
        _startScreen.SetActive(false);
        _howToPlayPanel.SetActive(false);
        _gameplayUI.SetActive(true); // your grid, UI, etc.
        _gameAssets.SetActive(true);

        GameManager.Instance.StartGame();
    }

    public void ShowHowToPlay()
    {
        _howToPlayPanel.SetActive(true);
        _startScreen.SetActive(false);
    }

    public void BackToStart()
    {
        _howToPlayPanel.SetActive(false);
        _startScreen.SetActive(true);
    }

    public void ToggleInGameHowToPlayText()
    {
        _inGameHowToPlayTextVisible = !_inGameHowToPlayTextVisible;

        _inGameHowToPlayText.SetActive(_inGameHowToPlayTextVisible);

        if (_inGameHowToPlayTextVisible )
        {
            GameManager.Instance.DisableInput();
        }
        else
        {
            GameManager.Instance.EnableInput();
        }
    }

    public void Restart()
    {
        _startScreen.SetActive(true);
        _gameplayUI.SetActive(false);
        _gameAssets.SetActive(false);
        _howToPlayPanel.SetActive(false);
        _inGameHowToPlayText.SetActive(false);

        GameManager.Instance.ResetGame();
    }
}
