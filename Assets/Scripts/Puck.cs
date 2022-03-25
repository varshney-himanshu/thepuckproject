using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Puck : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Vector3 _lastPos;

    public Vector3 _dragStartPos;
    public float _dragStartTime;
    private bool _isDragging;
    private float startPosX;
    private float startPosY;
    private Vector3 _velocity;
    private bool _disableTouch = false;

    private Vector3 _dragDirection;
    private Rigidbody2D _rb;

    private Vector3 _force;

    // private float _topSpeed = 10f;
    void Start()
    {
        _dragStartPos = this.transform.position;
        _dragStartTime = Time.time;
        _isDragging = false;
        _dragDirection = this.transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }

    public bool IsDragging()
    {
        return this._isDragging;
    }
    void Update()
    {
        if (_isDragging)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, -5);

            if (transform.position.x <= -8f)
            {
                transform.position = new Vector3(-8f, transform.position.y, 0);
            }
            if (transform.position.x >= 8f)
            {
                transform.position = new Vector3(8f, transform.position.y, 0);
            }
            if (transform.position.y <= -3.8f)
            {
                transform.position = new Vector3(transform.position.x, -4.3f, 0);
            }
            if (transform.position.y >= 3.8f)
            {
                transform.position = new Vector3(transform.position.x, 4.3f, 0);
            }
        }
    }

    void FixedUpdate()
    {
        _velocity = _rb.velocity;
    }


    void OnMouseUp()
    {
        if (_disableTouch)
        {
            return;
        }
        _isDragging = false;
        _rb.AddForce(_force * 100f, ForceMode2D.Impulse);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.ToLower() == "wall")
        {
            ReflectProjectile(_rb, collision.contacts[0].normal);
        }
    }

    private void ReflectProjectile(Rigidbody2D rb, Vector3 reflectVector)
    {
        _velocity = Vector3.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;
    }

    void OnMouseDown()
    {
        _velocity = new Vector3(0, 0, 0);
        _rb.velocity = new Vector3(0, 0, 0);
        if (_disableTouch)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;

            _dragStartPos = this.gameObject.transform.position;
            _dragStartTime = Time.time;
            _lastPos = _dragStartPos;
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            startPosX = mousePos.x - this.transform.localPosition.x;
            startPosY = mousePos.y - this.transform.localPosition.y;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("drag start");
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public Vector3 GetDragVelocity()
    {
        return _dragDirection;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dragDirection = this.transform.position - _lastPos;
        _force = _dragDirection;
        _lastPos = this.transform.position;
    }

}
