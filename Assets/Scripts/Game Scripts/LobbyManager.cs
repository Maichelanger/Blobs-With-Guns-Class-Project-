using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;

// This class represents a Lobby Manager that handles the creation, joining, and management of lobbies in a multiplayer game.
// It provides functionality for creating new lobbies, joining existing lobbies, retrieving the lobby list, sending heartbeat pings, and managing players in the lobby.
// The LobbyManager class also includes event handlers for various lobby-related events.
// It utilizes the Unity Services Lobbies API for lobby management and the Unity Services Authentication API for player authentication.
// The LobbyManager is a MonoBehaviour script that can be attached to a GameObject in the Unity scene.
// It is designed to work with the MultiplayerManager script, which handles the actual multiplayer functionality.
// The LobbyManager class is a singleton, meaning there can only be one instance of it in the game.
// It uses the Awake() method to initialize the Unity Authentication service asynchronously and the Update() method to handle periodic tasks such as sending heartbeat pings and retrieving the lobby list.
// The LobbyManager class also includes various methods for creating, joining, and managing lobbies, as well as getters for retrieving the currently joined lobby.
// Overall, the LobbyManager class serves as a central hub for managing multiplayer lobbies in the game.
public class LobbyManager : MonoBehaviour
{
    public const int MAX_PLAYERS = 2;
    public static LobbyManager Instance { get; private set; }
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

    // This class represents the event arguments for the lobby list changed event
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> LobbyList;
    }

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float LobbyListTimer;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    // Initializes the Unity Authentication service asynchronously
    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update()
    {
        HeartbeatHandler();
        LobbyListHandler();
    }

    // Handles the periodic retrieval of the lobby list
    private void LobbyListHandler()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn)
        {
            LobbyListTimer -= Time.deltaTime;
            if (LobbyListTimer <= 0f)
            {
                LobbyListTimer = 3f;
                LobbyList();
            }
        }
    }

    // Retrieves the list of available lobbies
    private async void LobbyList()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions()
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                LobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    // Handles the periodic sending of heartbeat pings to the lobby server
    private void HeartbeatHandler()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = 15f;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    // Checks if the local player is the host of the joined lobby
    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    // Creates a new lobby with the specified name and privacy setting
    public async void NewLobby(string nombreSala, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(nombreSala, MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            Debug.Log(joinedLobby.Name);
            MultiplayerManager.Instance.StartHost();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Attempts to join a lobby using the quick join feature
    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Attempts to join a lobby using the specified lobby ID
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Attempts to join a lobby using the specified lobby code
    public async void JoinWithCode(String codigo)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(codigo);
            MultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    // Deletes the currently joined lobby
    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    // Leaves the currently joined lobby
    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    // Kicks the specified player from the lobby if the local player is the host
    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    // Returns the currently joined lobby
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
