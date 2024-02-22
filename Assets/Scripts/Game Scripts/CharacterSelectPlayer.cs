using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// This class represents a player in the character select screen.
// It handles updating the player's information based on the player data received from the multiplayer manager.
public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private TextMeshPro playerNameTmpTxt;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private List<GameObject> skinList;

    // Subscribe to the event and initialize the player.
    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        readyGameObject.SetActive(false);
        UpdatePlayer();
    }

    // Update the player when the player data network list changes.
    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    // Update the player's information based on the player data.
    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            playerNameTmpTxt.text = playerData.playerName.ToString();
            /*
            foreach (GameObject skin in skinList)
            {
                skin.SetActive(false);
            }

            skinList[playerData.skinIndex].SetActive(true);
            */
            if (playerData.color != null)
            {
                GetComponentInChildren<SpriteRenderer>().color = playerData.color;
            }

            if (playerData.isPlayerReady)
            {
                readyGameObject.SetActive(true);
            }
        }
        else
        {
            Hide();
        }
    }

    // Show the player.
    private void Show()
    {
        gameObject.SetActive(true);
    }

    // Hide the player.
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
