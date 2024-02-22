using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class represents the game manager in the game. It is responsible for managing the game state, player readiness, and scene loading.
// It inherits from the NetworkBehaviour class, which allows it to synchronize data across the network.
// The GameManager class contains various events and methods to handle player readiness, scene loading, and network synchronization.
// It also has references to UI elements and game objects that are used in the game.
// Overall, this class plays a crucial role in coordinating the gameplay and network functionality of the game.
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnGameRadyChanged;
    public NetworkVariable<bool> gameReady;

    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject multiplayerMenuUI;
    [SerializeField] private GameObject newLobbyMenuUI;
    [SerializeField] private Transform playerPrefab;

    private bool isLocalPlayerReady;
    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        isLocalPlayerReady = false;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        gameReady = new NetworkVariable<bool>(false);
    }

    void Start()
    {
        OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        OnGameRadyChanged += GameManager_OnGameReadyChanged;
        mainMenuUI.SetActive(true);
        multiplayerMenuUI.SetActive(false);
        newLobbyMenuUI.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += GameManager_OnLoadEventCompleted;
        }
    }

    // This method is called when the load event for a scene is completed.
    // It checks if the loaded scene is the "GameScene" and if so, it instantiates player objects for each connected client.
    
    private void GameManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "GameScene")
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Transform playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<PlayerController>().PlayerSetUp(clientId);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            }
        }
    }

    // This method is called when the game ready state changes.
    // If the game is ready, it calls the GameManagerLoadSeceneServerRpc method to load the "GameScene".
    private void GameManager_OnGameReadyChanged(object sender, EventArgs e)
    {
        if (gameReady.Value)
        {
            GameManagerLoadSeceneServerRpc();
        }
    }

    // This method is a server RPC method that loads the "GameScene" on the server.
    [ServerRpc(RequireOwnership = false)]
    private void GameManagerLoadSeceneServerRpc()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    // This method is called when the local player ready state changes.
    // It calls the SetPlayerReadyServerRpc method to update the player ready state on the server.
    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        SetPlayerReadyServerRpc();
    }

    // This method returns the local player ready state.
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    // This method sets the local player ready state to true and calls the SetPlayerReadyServerRpc method to update the player ready state on the server.
    public void SetPlayerReady()
    {
        isLocalPlayerReady = true;
        SetPlayerReadyServerRpc();
        OnLocalPlayerReadyChanged(this, EventArgs.Empty);
    }

    // This method is a server RPC method that updates the player ready state on the server.
    // It checks if all clients are ready and if so, sets the game ready state to true.
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }
        if (allClientsReady)
        {
            gameReady.Value = true;
            OnGameRadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
