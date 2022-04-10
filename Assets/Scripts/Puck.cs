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

    private float _timeA = 0;
    private float _timeB = 0;
    private float _timeElapsed;
    private Vector3 _currentVelocity;
   [SerializeField] private float _timeExtendingFactor = 1f;

    private Vector3 _dragDirection;
    private Rigidbody2D _rb;

    private Vector3 _force;

    // private float _topSpeed = 10f;
    void Start()
    {
        _dragStartPos = this.transform.position;
        _dragStartTime = Time.time;
        _isDragging = false;
        _dragDirection = this.transform.position; // should be a zero vector at the start
        _rb = GetComponent<Rigidbody2D>();

       // _rb.velocity = new Vector3(0, 80, 0); // if any of the velocity component is more than or equal to 80 , ball will pass through walls
       // this bug was fixed by setting collision type as "continuous" instead of "discrete" on walls and strikers
    }

    public bool IsDragging()
    {
        return this._isDragging;
    }
    void Update()
    {
        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (_isDragging)
        {


            this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);

            //if (transform.position.x <= -1.8f)
            //{
            //    transform.position = new Vector3(-1.8f, transform.position.y, 0);
            //}
            //if (transform.position.x >= 1.8f)
            //{
            //    transform.position = new Vector3(1.8f, transform.position.y, 0);
            //}
            //if (transform.position.y <= -3.9f)
            //{
            //    transform.position = new Vector3(transform.position.x, -3.9f, 0);
            //}
            //if (transform.position.y >= 3.9f)
            //{
            //    transform.position = new Vector3(transform.position.x, 3.9f, 0);
            //}

            if (transform.position.x <= -1.8f)
            {
                transform.position = new Vector3(-1.8f, transform.position.y, 0);
            }
            if (transform.position.x >= 1.8f)
            {
                transform.position = new Vector3(1.8f, transform.position.y, 0);
            }
            if (transform.position.y <= -3.9f)
            {
                transform.position = new Vector3(transform.position.x, -3.9f, 0);
            }
            if (transform.position.y >= 3.9f)
            {
                transform.position = new Vector3(transform.position.x, 3.9f, 0);
            }
        }
        Debug.Log(mousePos + " :::::: " + _lastPos);

        if (mousePos == _lastPos)
        {
            _currentVelocity = Vector3.zero;
            Debug.Log("setting velocity to 0");
        }

       
    }

    void FixedUpdate()
    {
        _velocity = _rb.velocity;
    }


    void OnMouseUp()
    {
        //Debug.Log("on mouse up called");

        //Bug : when moving a striker a very low speed and releasing it , it still slides up or down. it should be at still position when releasing at a very low speed.
        if (_disableTouch)
        {
            return;
        }
        _isDragging = false;
        // _rb.AddForce(_currentVelocity * 1f, ForceMode2D.Impulse);

        
        _rb.velocity = _currentVelocity;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.tag);
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

        
        
        Debug.Log("on mouse down called");

        _velocity = new Vector3(0, 0, 0);
        _rb.velocity = new Vector3(0, 0, 0);
        _currentVelocity = new Vector3(0, 0, 0);
        Debug.Log("set velocity to 0 : " + _currentVelocity);

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
           // this.transform.position = mousePos;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("drag start");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("puck is not moving");
    }

    public Vector3 GetDragVelocity()
    {
        return _dragDirection;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        Debug.Log("on drag called");
        _dragDirection = this.transform.position - _lastPos;
        _timeElapsed = Time.time - _timeA + _timeExtendingFactor;
        _timeA = Time.time;
     //  Debug.Log(_dragDirection);
     if(_dragDirection.x == 0 && _dragDirection.y == 0 && _dragDirection.z == 0)
        {
            Debug.Log("coming here");
            _currentVelocity = new Vector3(0, 0, 0); 
        }
        else
        {
            _currentVelocity = _dragDirection / _timeElapsed;
        }
       // this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);


      //  Debug.Log("current velcoity : " + _currentVelocity);
       // _force = _currentVelocity;
        _lastPos = this.transform.position;
    }

}
