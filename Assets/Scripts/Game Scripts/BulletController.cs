using Unity.Netcode;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField] private float speed;

    public static BulletController LocalInstance { get; private set; }

    private Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
            rb = LocalInstance.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            rb.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
            Debug.Log(rb.position);
        }
    }
}
