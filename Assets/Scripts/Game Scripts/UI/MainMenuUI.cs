using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button MultiplayerGameButton;
    [SerializeField] private Button ExitGameButton;
    [SerializeField] private GameObject multiplayerMenuUI;

    void Start()
    {
        newGameButton.onClick.AddListener(() => NewGame());
        MultiplayerGameButton.onClick.AddListener(() => NewMultiplayerGame());
        ExitGameButton.onClick.AddListener(Application.Quit);
    }

    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void NewMultiplayerGame()
    {
        multiplayerMenuUI.SetActive(true);
    }
}
