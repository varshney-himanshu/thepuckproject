using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class striker : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 _velocity;
    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
       // _velocity = _rb.velocity;
        
    }

    

         void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.ToLower() == "wall")
        //{
        //    ReflectProjectile(_rb, collision.contacts[0].normal);
        //}

       // _rb.AddForce(collision.gameObject.GetComponent<Rigidbody2D>().velocity * 250f, ForceMode2D.Impulse);

       ;

    }

}
