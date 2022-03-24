using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Puck : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Vector3 _lastPos;

    public Vector3 _dragStartPos;
    public float _dragStartTime;

    public Vector3 _dragEndPos;
    public float _dragEndTime;

    public GameObject[] validPositions;
    private bool _isDragging;
    private Vector3 resetPosition;
    private float startPosX;
    private float startPosY;
    Vector3 direction;
    private Vector3 _velocity;
    private bool _disableTouch = false;

    private Vector3 _dragDirection;
    private Rigidbody2D _rb;

    private Vector3 _force;

    private float _topSpeed = 10f;

    void Start()
    {
        _dragStartPos = this.transform.position;
        _dragStartTime = Time.time;
        _isDragging = false;
        _dragDirection = this.transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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

    // void FixedUpdate()
    // {
    //     // Debug.Log(_force);
    //     _rb.velocity = _force;
    // }
    // void FixedUpdate()
    // {
    //     GetComponent<Rigidbody2D>().velocity = _velocity;

    //     if (transform.position.x <= -8f)
    //     {
    //         transform.position = new Vector3(-8f, transform.position.y, 0);
    //         _velocity = new Vector3(-_velocity.x, _velocity.y, _velocity.z);
    //     }
    //     if (transform.position.x >= 8f)
    //     {
    //         transform.position = new Vector3(8f, transform.position.y, 0);
    //         _velocity = new Vector3(-_velocity.x, _velocity.y, _velocity.z);
    //     }
    //     if (transform.position.y <= -4f)
    //     {
    //         transform.position = new Vector3(transform.position.x, -4.3f, 0);
    //         _velocity = new Vector3(_velocity.x, _velocity.y, _velocity.z);
    //     }
    //     if (transform.position.y >= 4f)
    //     {
    //         transform.position = new Vector3(transform.position.x, 4.3f, 0);
    //         _velocity = new Vector3(_velocity.x, -_velocity.y, _velocity.z);
    //     }                                                    //  GetComponent<Rigidbody2D>().velocity = this.gameObject.transform.position.normalized;     //  GetComponent<Rigidbody2D>().AddForce(v.normalized * 10f);    
    // }

    void OnMouseUp()
    {
        if (_disableTouch)
        {
            return;
        }
        _isDragging = false;
        GetComponent<Rigidbody2D>().isKinematic = false;

        direction = (_dragStartPos - this.gameObject.transform.position);
        _dragEndTime = Time.time;
        _dragEndPos = this.gameObject.transform.position;

        _velocity = (_dragEndPos - _dragStartPos) / (_dragEndTime - _dragStartTime);
        _velocity = Vector3.Scale(GetAbsVector3(_velocity), _dragDirection);
        // this.GetComponent<Rigidbody2D>().AddForce(_dragDirection, ForceMode2D.Impulse);

        // if (_force.magnitude > _topSpeed)
        //     _force = _force.normalized * _topSpeed;
        Debug.Log(_force);
        _rb.AddForce(_force, ForceMode2D.Impulse);


    }

    Vector3 GetUnitVector(Vector3 vector)
    {
        float x = vector.x;
        float y = vector.y;
        float z = vector.z;
        x = x < 0 ? -1 : 1;
        y = y < 0 ? -1 : 1;
        z = z < 0 ? -1 : 1;
        return new Vector3(x, y, z);
    }

    Vector3 GetAbsVector3(Vector3 v)
    {
        float x = v.x;
        float y = v.y;
        float z = v.z;

        x = Mathf.Abs(x);
        y = Mathf.Abs(y);
        z = Mathf.Abs(z);
        return new Vector3(x, y, z);
    }

    void OnMouseDown()
    {
        _velocity = new Vector3(0, 0, 0);
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

    public void OnDrag(PointerEventData eventData)
    {
        _dragDirection = this.transform.position - _lastPos;
        // _dragDirection = GetUnitVector(_dragDirection);
        _force = _dragDirection;
        _lastPos = this.transform.position;
    }


}
