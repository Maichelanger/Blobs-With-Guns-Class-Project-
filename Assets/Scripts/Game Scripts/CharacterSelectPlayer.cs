using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{


    [SerializeField] private int playerIndex;
    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerManager_OnPlayerDataNetworkListChanged;
        
        UpdatePlayer();
    }

    private void MultiplayerManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }



    private void UpdatePlayer()
    {
        if(MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
        }
        else
        {
            Hide();
        }
    }



    private void Show()
    {
        gameObject.SetActive(true);
    }


    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
