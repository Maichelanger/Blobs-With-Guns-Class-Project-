using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public static event EventHandler OnAnyPlayerSpawed;

    public static PlayerController LocalInstance { get; private set; }

    [SerializeField] private float velocidad;
    [SerializeField] private float fuerzaSalto;
    [SerializeField] private GameObject bulletSpawner;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject weapon;
    [SerializeField] private List<GameObject> skinList;

    private Rigidbody2D rb;
    //private Animator anim;
    private PlayerData localPlayerData;
    private GameObject localSkin;
    private int skinIndex = 0;
    private Camera mainCamera;

    private bool right;



    private void Awake()
    {
        right = true;
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

        //   localPlayerData.playerName = MultiplayerManager.Instance.GetPlayerName();

        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponentInChildren<Animator>();
        localSkin = skinList[skinIndex];
        weapon.transform.SetParent(localSkin.transform);

        this.transform.SetPositionAndRotation(new Vector3(-1f, 0f, 0), Quaternion.identity);

    }


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawed?.Invoke(this, EventArgs.Empty);
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        Move();
        CheckFire();
    }

    public void Move()
    {
        if (this.transform.rotation == Quaternion.identity) // Desplazamiento hacia la derecha
        {
            rb.velocity = (transform.right * velocidad * Input.GetAxis("Horizontal")) +
                    (transform.up * rb.velocity.y);
        }
        else
        { // Desplazamiento hacia la izuquierda
            rb.velocity = -(transform.right * velocidad * Input.GetAxis("Horizontal")) +
                (transform.up * rb.velocity.y);
        }

        // Actualizamos la posición
        this.transform.SetLocalPositionAndRotation(this.transform.position, this.transform.rotation);


        // Rotamos el jugador en función de la pulsación de teclas.
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.rotation = Quaternion.identity;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // Configuración del salto.
        if (Input.GetKeyDown(KeyCode.UpArrow) && (Mathf.Abs(rb.velocity.y) < 0.2f))
        {
            rb.AddForce(transform.up * fuerzaSalto);
        }

        // Actualizamos la animación
        //anim.SetFloat("velocidadX", Mathf.Abs(rb.velocity.x));
        //anim.SetFloat("velocidadY", rb.velocity.y);

        Camera.main.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
    }


    public void CheckFire()
    {
        if (Input.GetButtonDown("Jump"))
            FireServerRpc(localSkin.transform.rotation, bulletSpawner.transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void FireServerRpc(Quaternion rotation, Vector3 position)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        NetworkObject bulletNetwork = bullet.GetComponent<NetworkObject>();
        bulletNetwork.Spawn(true);
        bullet.transform.rotation = rotation;
        bulletNetwork.transform.rotation = rotation;
        bullet.transform.position = position;
        bulletNetwork.transform.position = position;


        Destroy(bullet, 2f);
    }





    public void SelectSkin(int skinIndex)
    {
        if (!IsOwner)
        {
            return;
        }
        // Asignamos los elementos a la nueva skin
        this.skinIndex = skinIndex;
        GameObject newSkin = skinList[skinIndex];
        newSkin.transform.rotation = localSkin.transform.rotation;
        weapon.transform.SetParent(newSkin.transform);


        //Desactivamos la anterior
        localSkin.SetActive(false);
        //Activamos el nuevo
        newSkin.SetActive(true);

        // Reasignamos nombres.
        localSkin = newSkin;

        // Actualizamos la animación
        //anim = GetComponentInChildren<Animator>();
    }




}
