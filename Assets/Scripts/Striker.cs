using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Striker : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public Vector3 _lastPos;

    public Vector3 _dragStartPos;
    public float _dragStartTime;
    private bool _isDragging;
    private float startPosX;
    private float startPosY;
    private Vector3 _velocity;
    private bool _disableTouch = false;

    private Vector3 _dragInstantVelocity;
    private Rigidbody2D _rb;

    [SerializeField] private float FORCE_MULTIPLIER = 100f;


    [SerializeField] GameObject _southWall;
    [SerializeField] GameObject _northWall;
    [SerializeField] GameObject _eastWall;
    [SerializeField] GameObject _westWall;

    float _northBound;
    float _southBound;
    float _eastBound;
    float _westBound;

    float STRIKER_WIDTH;
    float STRIKER_HEIGHT;
    float WALL_WIDTH;

    void Awake()
    {
        _dragStartPos = this.transform.position;
        _dragStartTime = Time.time;
        _isDragging = false;
        _dragInstantVelocity = this.transform.position;
        _rb = GetComponent<Rigidbody2D>();

        _northBound = _northWall.transform.position.y - (_northWall.GetComponent<RectTransform>().rect.height) / 2 * Mathf.Abs(_northWall.transform.localScale.y);
        _southBound = _southWall.transform.position.y + (_southWall.GetComponent<RectTransform>().rect.height) / 2 * Mathf.Abs(_southWall.transform.localScale.y);
        _eastBound = _eastWall.transform.position.x - (_eastWall.GetComponent<RectTransform>().rect.width) / 2 * Mathf.Abs(_eastWall.transform.localScale.x);
        _westBound = _westWall.transform.position.x + (_westWall.GetComponent<RectTransform>().rect.width) / 2 * Mathf.Abs(_westWall.transform.localScale.x);


        STRIKER_WIDTH = this.gameObject.GetComponent<RectTransform>().rect.width * Mathf.Abs(transform.localScale.x);
        STRIKER_HEIGHT = this.gameObject.GetComponent<RectTransform>().rect.height * Mathf.Abs(transform.localScale.y);


    }

    void Start() { }


    void CheckIfStrikerIsOutsideBounds()
    {
        if (transform.position.x - STRIKER_WIDTH / 2 <= _westBound)
        {
            transform.position = new Vector3(_westBound + STRIKER_WIDTH / 2, transform.position.y, 0);
        }
        if (transform.position.x + STRIKER_WIDTH / 2 >= _eastBound)
        {
            transform.position = new Vector3(_eastBound - STRIKER_WIDTH / 2, transform.position.y, 0);
        }
        if (transform.position.y - STRIKER_WIDTH / 2 <= _southBound)
        {
            transform.position = new Vector3(transform.position.x, _southBound + STRIKER_WIDTH / 2, 0);
        }
        if (transform.position.y + STRIKER_WIDTH / 2 >= _northBound)
        {
            transform.position = new Vector3(transform.position.x, _northBound - STRIKER_WIDTH / 2, 0);
        }
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

            // if (transform.position.x <= -1.8f)
            // {
            //     transform.position = new Vector3(-1.8f, transform.position.y, 0);
            // }
            // if (transform.position.x >= 1.8f)
            // {
            //     transform.position = new Vector3(1.8f, transform.position.y, 0);
            // }
            // if (transform.position.y <= -3.9f)
            // {
            //     transform.position = new Vector3(transform.position.x, -3.9f, 0);
            // }
            // if (transform.position.y >= 3.9f)
            // {
            //     transform.position = new Vector3(transform.position.x, 3.9f, 0);
            // }
        }

        CheckIfStrikerIsOutsideBounds();
    }

    void FixedUpdate()
    {
        _velocity = _rb.velocity;
    }

    public Vector3 GetDragVelocity()
    {
        return _dragInstantVelocity;
    }

    void OnMouseUp()
    {
        if (_disableTouch)
        {
            return;
        }
        _isDragging = false;
        _rb.AddForce(_dragInstantVelocity * FORCE_MULTIPLIER, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.tag);
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


    public void OnDrag(PointerEventData eventData)
    {
        _dragInstantVelocity = this.transform.position - _lastPos;
        _lastPos = this.transform.position;
    }

}