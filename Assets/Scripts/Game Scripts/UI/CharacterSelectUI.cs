using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button skin1Button;
    [SerializeField] private Button skin2Button;
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text lobbyCode;

    private void Awake()
    {
       // lobbyName.text = LobbyManager.Instance.GetLobby().Name;
       // lobbyCode.text = LobbyManager.Instance.GetLobby().LobbyCode;

        mainMenuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MenuScene");
        });

        readyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SetPlayerReady();
            readyButton.interactable = false;
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
                PlayerController.LocalInstance.transform.SetPositionAndRotation(new Vector3(-7f, 1f, 0), Quaternion.identity);
            }
        });

        skin1Button.onClick.AddListener(() =>
        {
            //PlayerController.LocalInstance.SelectSkin(1);
        });

        skin2Button.onClick.AddListener(() =>
        {
            //PlayerController.LocalInstance.SelectSkin(0);
        });

    }
}
