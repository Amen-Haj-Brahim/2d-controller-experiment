using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pad : MonoBehaviour
{
    public float bounce = 20f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
           rb.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
        }
    }
}
