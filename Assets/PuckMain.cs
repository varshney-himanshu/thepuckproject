using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckMain : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D _rb;
    private Vector3 _velocity;
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
    }
    void FixedUpdate()
    {
        _velocity = _rb.velocity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.ToLower() == "wall")
        {
            ReflectProjectile(_rb, collision.contacts[0].normal);
        }

        if (collision.gameObject.tag.ToLower() == "player")
        {

            Puck puck = collision.gameObject.GetComponent<Puck>();
            if (puck.IsDragging())
            {
                _rb.AddForce(puck.GetDragVelocity() * 70f, ForceMode2D.Impulse);
            }
        }

    }


    private void ReflectProjectile(Rigidbody2D rb, Vector3 reflectVector)
    {
        _velocity = Vector3.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;

    }

}
