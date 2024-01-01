using UnityEngine;

public class gravityscript : MonoBehaviour
{
    public movement m;
    public float gravity;
    public bool grounded;
    Rigidbody2D rb;
    void Awake()
    {
        rb =GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    void FixedUpdate()
    {
        grounded = m.OnGround;
        if (!grounded && Mathf.Abs(rb.velocity.y) < 0.3f)
        {
            rb.gravityScale = .5f;
        }
        else
        {
            rb.gravityScale = gravity;
        }
        if (rb.velocity.y < 0 && !grounded)
        {
            rb.gravityScale *= 2.5f;
        }
        else if (grounded)
        {
            rb.gravityScale = gravity;
        }
        if(rb.velocity.y > 0)
        {
            Debug.Log(rb.gravityScale);
            rb.gravityScale*=1.5f;
        }
    }
}