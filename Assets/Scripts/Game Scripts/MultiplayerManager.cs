using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class represents the Multiplayer Manager in the game.
// It handles various multiplayer functionalities such as hosting a game, joining a game,
// managing player data, and handling player connections and disconnections.

// It also includes server RPC methods for setting player names, player IDs, player skins,
// player colors, and player ready states.

// The class uses the Unity.Netcode library for networking and Unity.Services.Authentication for player authentication.
// It also includes events for notifying when a player is trying to join a game, when a player fails to join a game, and when the player data network list changes.
public class MultiplayerManager : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string PLAYER_NAME = "NombreJugador";
    public static MultiplayerManager Instance { get; private set; }
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler ConnectionApprovalCallBack;

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    // Get the player name
    public string GetPlayerName()
    {
        return playerName;
    }

    // Set the player name
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_NAME, playerName);
    }

    // This method is called when the player data network list changes. It invokes the OnPlayerDataNetworkListChanged event.
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Start hosting a multiplayer game
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
    }

    // This method removes the player data associated with the disconnected client from the playerDataNetworkList.
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    // This method is called when a client successfully connects to the server.
    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            // Set the client ID of the connected player
            clientId = clientId,
            skinIndex = 0,
            color = Color.white
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    // This method is called when a client tries to connect to the server. It checks if the maximum number of players has been reached and approves or rejects the connection accordingly.
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "La Partida está completa";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    // Start joining a multiplayer game
    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    // This method is called when a client successfully connects to the server.
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    // This method sets the player name for the client associated with the given server RPC parameters.
    private void SetPlayerNameServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerName = name;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // This method sets the player ID for the client associated with the given server RPC parameters.
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.playerId = playerId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // This method is called when a client disconnects from the server. It invokes the OnFailedToJoinGame event.
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    // Check if a player index is connected
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return (playerIndex < playerDataNetworkList.Count);
    }

    // Get the player data index from a client ID
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
                return i;
        }
        return -1;
    }

    // Get the player data from a client ID
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
                return playerData;
        }
        return default;
    }

    // Get the local player data
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    // Get the player data from a player index
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    // Get the player skin index from a player index
    public int GetPlayerSkinIndexFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex].skinIndex;
    }

    // Set the player skin
    public void SetPlayerSkin(int skinIndex)
    {
        ChangePlayerSkinServerRpc(skinIndex);
    }

    // This method changes the player skin index for the client associated with the given server RPC parameters.
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerSkinServerRpc(int skinIndex, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.skinIndex = skinIndex;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // Set the player color
    public void SetPlayerColor(Color color)
    {
        SetPlayerColorServerRpc(color);
    }

    // This method sets the player color for the client associated with the given server RPC parameters.
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerColorServerRpc(Color color, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.color = color;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // Set the player ready state
    public void IsPlayerReady(bool isReady)
    {
        IsPlayerReadyServerRpc(isReady);
    }

    [ServerRpc(RequireOwnership = false)]
    // This method sets the player ready state for the client associated with the given server RPC parameters.
    private void IsPlayerReadyServerRpc(bool isReady, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.isPlayerReady = isReady;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    // Kick a player from the game
    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
