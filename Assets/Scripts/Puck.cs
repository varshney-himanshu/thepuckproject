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

    float _northBound;
    float _southBound;
    float _eastBound;
    float _westBound;

    float PUCK_WIDTH;
    float PUCK_HEIGHT;

    void Awake()
    {
        _northBound = _northWall.transform.position.y - (_northWall.GetComponent<RectTransform>().rect.height) / 2 * Mathf.Abs(_northWall.transform.localScale.y);
        _southBound = _southWall.transform.position.y + (_southWall.GetComponent<RectTransform>().rect.height) / 2 * Mathf.Abs(_southWall.transform.localScale.y);
        _eastBound = _eastWall.transform.position.x - (_eastWall.GetComponent<RectTransform>().rect.width) / 2 * Mathf.Abs(_eastWall.transform.localScale.x);
        _westBound = _westWall.transform.position.x + (_westWall.GetComponent<RectTransform>().rect.width) / 2 * Mathf.Abs(_westWall.transform.localScale.x);

        PUCK_WIDTH = this.gameObject.GetComponent<RectTransform>().rect.width * Mathf.Abs(transform.localScale.x);
        PUCK_HEIGHT = this.gameObject.GetComponent<RectTransform>().rect.height * Mathf.Abs(transform.localScale.y);


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

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.ToLower() == "wall")
        {
            ReflectProjectile(_rb, collision.contacts[0].normal);
        }

        if (collision.gameObject.tag.ToLower() == "player")
        {
            Striker striker = collision.gameObject.GetComponent<Striker>();
            if (striker.IsDragging())
            {
                _rb.AddForce(striker.GetDragVelocity() * FORCE_MULTIPIER, ForceMode2D.Impulse);
            }
        }
    }


    private void ReflectProjectile(Rigidbody2D rb, Vector3 reflectVector)
    {
        _velocity = Vector3.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;
    }

}
