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

    private Vector3 _force = new Vector3(0f,0f,0f);

    private Vector3 puckLine = new Vector3(0f, 0f, 0f);

    private Vector3 oldPosition = new Vector3(0f, 0f, 0f);

    private Vector3 newPosition = new Vector3(0f, 0f, 0f);

    [SerializeField] GameObject sw;
    [SerializeField] GameObject nw;
    [SerializeField] GameObject ew;
    [SerializeField] GameObject ww;

    float northBound;
    float southBound;
    float eastBound;
    float westBound;

    float PUCK_WIDTH;
    float WALL_WIDTH;


    // private float _topSpeed = 10f;

    void Awake()
    {
        northBound = nw.transform.position.y - (nw.GetComponent<RectTransform>().rect.height)/2 * Mathf.Abs(nw.transform.localScale.x);
        southBound = sw.transform.position.y + (sw.GetComponent<RectTransform>().rect.height)/2 * Mathf.Abs(sw.transform.localScale.x);
        eastBound = ew.transform.position.x - (ew.GetComponent<RectTransform>().rect.width)/2 * Mathf.Abs(ew.transform.localScale.x);
        westBound = ww.transform.position.x + (ww.GetComponent<RectTransform>().rect.width)/2 * Mathf.Abs(ww.transform.localScale.x);

        PUCK_WIDTH = this.gameObject.GetComponent<RectTransform>().rect.width * Mathf.Abs(transform.localScale.x);
    }
    void Start()
    {
        _dragStartPos = this.transform.position;
        _dragStartTime = Time.time;
        _isDragging = false;
        _dragDirection = this.transform.position;
        _rb = GetComponent<Rigidbody2D>();

        

        

    //  RectTransform rt = this.gameObject.GetComponent<RectTransform>();
     // puckWidth = rt.rect.width * Mathf.Abs(transform.localScale.x);
        Debug.Log("nw  " + northBound);
        Debug.Log("sw  " + southBound);
        Debug.Log("ew  " + eastBound);
        Debug.Log("ww  " + westBound);


    }

    void Update()
    {
        if (_isDragging)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            oldPosition = newPosition;
            newPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);
            this.gameObject.transform.localPosition = newPosition;
            //_rb.MovePosition(newPosition);
            //puckLine = newPosition - oldPosition;
            Debug.Log(transform.position.x - Mathf.Abs(transform.localScale.y));

        }
        
        if (transform.position.x - PUCK_WIDTH/2 <= westBound)
        {

            
            transform.position = new Vector3(westBound  + PUCK_WIDTH/2,   transform.position.y, 0);

            
        }
        if (transform.position.x + PUCK_WIDTH/2 >= eastBound)
        {
            Debug.Log("baharr  jaa  rha  h  kanjar");
             transform.position = new Vector3(eastBound - PUCK_WIDTH / 2, transform.position.y, 0);

            //_rb.MovePosition(new Vector3(eastBound - Mathf.Abs(transform.localScale.y) / 2, transform.position.y, 0));
        }
        if (transform.position.y - PUCK_WIDTH / 2 <= southBound)
        {
            Debug.Log("baharr  jaa  rha  h  kanjar");
            transform.position = new Vector3(transform.position.x, southBound + PUCK_WIDTH / 2, 0);

            //_rb.MovePosition(new Vector3(transform.position.x, southBound + Mathf.Abs(transform.localScale.y) / 2, 0));
        }
        if (transform.position.y + PUCK_WIDTH / 2 >= northBound)
        {
            Debug.Log("baharr  jaa  rha  h  kanjar");
            transform.position = new Vector3(transform.position.x, northBound - PUCK_WIDTH / 2, 0);
            //_rb.MovePosition(new Vector3(transform.position.x, northBound - Mathf.Abs(transform.localScale.y), 0));

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
        _rb.AddForce(_force * 250f, ForceMode2D.Impulse);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.ToLower() == "wall")
        //{
        //    ReflectProjectile(_rb, collision.contacts[0].normal);
        //}

        //if(_isDragging)
        //{
        //  _rb.MovePosition(_lastPos);
        //}
        Debug.Log("colision detexted by puck");
      // _isDragging = false;
       // _rb.MovePosition(_lastPos);
        _velocity = Vector3.Reflect(_velocity, collision.contacts[0].normal);
        _rb.velocity = _velocity;
       
    }

    

    private void ReflectProjectile(Rigidbody2D rb, Vector3 reflectVector)
    {
        _velocity = Vector3.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;
    }

    void OnMouseDown()
    {
        
       // _velocity = new Vector3(0, 0, 0);
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
       // Vector3 mousePos;
       // mousePos = Input.mousePosition;
       // mousePos = Camera.main.ScreenToWorldPoint(mousePos);
       // _lastPos = mousePos;
       // _rb.MovePosition(mousePos);
    }

}
