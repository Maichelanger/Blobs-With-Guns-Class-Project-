using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private float velocidad;
    [SerializeField] private float fuerzaSalto;
    //[SerializeField] private GameObject bulletSpawner;
    //[SerializeField] private GameObject bulletPrefab;
    //[SerializeField] private GameObject weapon;
    [SerializeField] private List<GameObject> skinList;

    private Rigidbody2D rb;
    //private Animator anim;
    private PlayerData localPlayerData;
    private GameObject localSkin;
    private Camera cam;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        PlayerSetUp(OwnerClientId);
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!IsOwner)
            return;

        Move();
        CheckFire();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            cam = FindObjectOfType<Camera>();
        }
        localPlayerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    // Callback method for handling client disconnection
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        throw new NotImplementedException();
    }

    // Sets the player's skin based on the given index
    public void SetPlayerSkin(int skin)
    {
        for (int i = 0; i < skinList.Count; i++)
        {
            if (i != skin)
            {
                skinList[i].SetActive(false);
            }
        }

        localSkin = skinList[skin];
        localSkin.SetActive(true);
        //anim = localSkin.GetComponent<Animator>();
        //weapon.transform.SetParent(localSkin.transform);
    }

    // Sets the player's color to the given color
    public void SetPlayerColor(Color color)
    {
        GetComponentInChildren<SpriteRenderer>().color = color;
    }

    // Sets up the player based on the given client ID
    public void PlayerSetUp(ulong clientId)
    {
        localPlayerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(clientId);
        SetPlayerSkin(localPlayerData.skinIndex);
        SetPlayerColor(localPlayerData.color);
    }

    // Moves the player based on input
    public void Move()
    {
        if (this.transform.rotation == Quaternion.identity)
        {
            rb.velocity = (transform.right * velocidad * Input.GetAxis("Horizontal")) +
                    (transform.up * rb.velocity.y);
        }
        else
        {
            rb.velocity = -(transform.right * velocidad * Input.GetAxis("Horizontal")) +
                (transform.up * rb.velocity.y);
        }

        this.transform.SetLocalPositionAndRotation(this.transform.position, this.transform.rotation);

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.rotation = Quaternion.identity;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && (Mathf.Abs(rb.velocity.y) < 0.2f))
        {
            rb.AddForce(transform.up * fuerzaSalto);
        }

        //anim.SetFloat("velocidadX", Mathf.Abs(rb.velocity.x));
        //anim.SetFloat("velocidadY", rb.velocity.y);

        cam.transform.SetPositionAndRotation(new Vector3(this.transform.position.x + 2, this.transform.position.y + 2, -10), Quaternion.identity);
    }

    // Checks if the player should fire and triggers the server RPC
    public void CheckFire()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
            //FireServerRpc(localSkin.transform.rotation, bulletSpawner.transform.position);
    }

    // Server RPC method for firing a bullet
    [ServerRpc(RequireOwnership = false)]
    private void FireServerRpc(Quaternion rotation, Vector3 position, ServerRpcParams serverRpcParams = default)
    {
        //GameObject bullet = Instantiate(bulletPrefab, position, rotation);

        //NetworkObject bulletNetwork = bullet.GetComponent<NetworkObject>();
        //bulletNetwork.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);

        //Destroy(bullet, 2f);
    }
}
