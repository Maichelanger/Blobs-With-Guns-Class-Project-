using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This class represents the user interface for character selection in the game.
public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    //[SerializeField] private Button skin1Button;
    //[SerializeField] private Button skin2Button;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyCode;
    [SerializeField] private Button noColorButton;
    [SerializeField] private Button whiteButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button purpleButton;

    // This Awake() method is called when the GameObject this script is attached to is initialized.
    // It sets up event listeners and assigns click handlers to various buttons in the UI.
    // It also initializes the lobby name and lobby code text fields with the corresponding values from the LobbyManager.
    // Finally, it subscribes to the OnLocalPlayerReadyChanged event of the GameManager and checks if the local player is ready.
    // If the local player is ready and the current instance is the host, it loads the "GameScene".
    private void Awake()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;

        lobbyName.text = LobbyManager.Instance.GetLobby().Name;
        lobbyCode.text = LobbyManager.Instance.GetLobby().LobbyCode;

        mainMenuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MenuScene");
        });

        readyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReady();
            MultiplayerManager.Instance.IsPlayerReady(true);
            readyButton.interactable = false;
            var colors = readyButton.colors;
            colors.disabledColor = new Color(0.5f, 1, 0, 1);
            Debug.Log("Player Ready: " + GameManager.Instance.IsLocalPlayerReady());
            Debug.Log("Game Ready: " + GameManager.Instance.gameReady.Value);
        });
        /*
        skin1Button.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerSkin(0);
        });

        skin2Button.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerSkin(1);
        });
        */
        noColorButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(1, 1, 1, 1f));
        });

        whiteButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(0, 0, 0, 1));
        });

        blueButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(0, 0, 1, 1));
        });

        redButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(1, 0, 0, 1));
        });

        greenButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(0, 1, 0, 1));
        });

        purpleButton.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.SetPlayerColor(new Color(1, 0, 1, 1));
        });
    }

    // This method is called when the local player's ready state changes.
    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (NetworkManager.Singleton.IsHost && GameManager.Instance.gameReady.Value)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}
