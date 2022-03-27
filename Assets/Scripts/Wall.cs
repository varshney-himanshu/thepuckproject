using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.ToLower() == "wall")
        //{
        //    ReflectProjectile(_rb, collision.contacts[0].normal);
        //}


        Debug.Log("collision detected");

        

    }

}
