using UnityEngine;

public class BoxController : MonoBehaviour
{
    [SerializeField] private int life;

    private bool broken = false;
    private Animator anim;
    private Rigidbody2D rb;

    public void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Life", life);
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (life > 0)
        {
            life--;
            anim.SetInteger("Life", life);
            anim.SetTrigger("Hit");
        }

        if (life == 0)
        {
            anim.SetBool("Break", true);
            broken = true;
        }
    }

    public void Update()
    {
        if (broken)
        {
            Destroy(transform.gameObject);
        }
    }
}
