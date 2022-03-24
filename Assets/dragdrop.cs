using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class dragdrop : MonoBehaviour
{
    public Vector3 prevPos;

    public GameObject[] validPositions;
    private bool moving;
    private Vector3 resetPosition;
    private float startPosX;
    private float startPosY;
    Vector3 direction;
    private Vector3 v;
    private bool disableTouch = false;
    // SoundHandler soundHandler;
    public void SetCardSprite(Sprite newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public void Start()
    {
        // soundHandler = FindObjectOfType<SoundHandler>();
        prevPos = this.transform.position;
    }
    public void Update()
    {
        if (moving)
        {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, -5);

            Debug.Log(GetComponent<Rigidbody2D>().velocity);

            v = this.gameObject.transform.localPosition;

        }
    }
    void FixedUpdate()
    {

        GetComponent<Rigidbody2D>().velocity = -direction;     //  GetComponent<Rigidbody2D>().AddForce(v.normalized * 10f);    
                                                               //  GetComponent<Rigidbody2D>().velocity = this.gameObject.transform.position.normalized;     //  GetComponent<Rigidbody2D>().AddForce(v.normalized * 10f);    
    }
    public void OnMouseUp()
    {
        if (disableTouch)
        {
            return;
        }
        moving = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
        // Debug.Log( "new pos: " + this.gameObject.transform.position); Debug.Log("prevpos: " + prevPos);
        Debug.Log("direction: " + (prevPos - this.gameObject.transform.position));
        direction = (prevPos - this.gameObject.transform.position);
    }

    private void OnMouseDrag()
    {

    }

    void OnMouseDown()
    {
        // GetComponent<Rigidbody2D>().isKinematic = true; 
        // soundHandler.PlayCardPickSound();
        Debug.Log("mouse down");

        if (disableTouch)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {

            Debug.Log("mouse down");
            prevPos = this.gameObject.transform.position;
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            startPosX = mousePos.x - this.transform.localPosition.x;
            startPosY = mousePos.y - this.transform.localPosition.y;
            moving = true;
        }
    }


}