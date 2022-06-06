using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _velocity;
    [SerializeField] private float FORCE_MULTIPIER = 70f;

    [SerializeField] GameObject _southWall;
    [SerializeField] GameObject _northWall;
    [SerializeField] GameObject _eastWall;
    [SerializeField] GameObject _westWall;


    float PUCK_WIDTH;
    float PUCK_HEIGHT;

    Boundary _boundary;

    void Awake()
    {

        PUCK_WIDTH = this.gameObject.GetComponent<RectTransform>().rect.width * Mathf.Abs(transform.localScale.x);
        PUCK_HEIGHT = this.gameObject.GetComponent<RectTransform>().rect.height * Mathf.Abs(transform.localScale.y);

        _boundary = new Boundary(_northWall.transform.position.y - PUCK_HEIGHT / 2, _southWall.transform.position.y + PUCK_HEIGHT / 2, _westWall.transform.position.x + PUCK_WIDTH / 2, _eastWall.transform.position.x - PUCK_WIDTH / 2);
    }
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
    }
    void FixedUpdate()
    {
        _velocity = _rb.velocity;
    }

    void Update()
    {

        if (_rb.position.x > _boundary.right || _rb.position.x < _boundary.left || _rb.position.y > _boundary.top || _rb.position.y < _boundary.bottom)
        {
            Debug.Log("outside bounds!!!");
            transform.position = (new Vector3(Mathf.Clamp(_rb.position.x, _boundary.left, _boundary.right), Mathf.Clamp(_rb.position.y, _boundary.bottom, _boundary.top), 0));
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.ToLower() == "wall")
        //{
        //    ReflectProjectile(_rb, collision.contacts[0].normal);
        //}

        //if (collision.gameObject.tag.ToLower() == "player")
        //{
        //    Striker striker = collision.gameObject.GetComponent<Striker>();
        //    if (striker.IsDragging())
        //    {
        //        _rb.AddForce(striker.GetDragVelocity() * FORCE_MULTIPIER, ForceMode2D.Impulse);
        //    }
        //}
    }


    private void ReflectProjectile(Rigidbody2D rb, Vector3 reflectVector)
    {
        _velocity = Vector3.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;
    }

}
