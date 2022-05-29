using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerBot : MonoBehaviour
{

    [SerializeField] private float _maxMovementSpeed = 10f;
    private float _movementSpeed;
    private Rigidbody2D _rb;
    private Vector2 _startingPos;

    [SerializeField] Rigidbody2D _rbPuck;

    [SerializeField] GameObject _southWall;
    [SerializeField] GameObject _northWall;
    [SerializeField] GameObject _eastWall;
    [SerializeField] GameObject _westWall;

    [SerializeField] GameObject _centerPos;



    float _centerY;
    float _centerX;


    float STRIKER_WIDTH;
    float STRIKER_HEIGHT;

    private Vector2 _targetPos;

    private bool _isInOpponentsHalf = true;

    private float _offsetXMovement;

    private Boundary _boundary;
    private Boundary _puckBoundary;

    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _startingPos = this._rb.position;

        STRIKER_WIDTH = this.gameObject.GetComponent<RectTransform>().rect.width * Mathf.Abs(transform.localScale.x);
        STRIKER_HEIGHT = this.gameObject.GetComponent<RectTransform>().rect.height * Mathf.Abs(transform.localScale.y);

        _centerY = _centerPos.transform.position.y;
        _centerX = _centerPos.transform.position.x;

        this._boundary = new Boundary(_northWall.transform.position.y - STRIKER_HEIGHT / 2, _centerY + STRIKER_HEIGHT / 2, _westWall.transform.position.x + STRIKER_WIDTH / 2, _eastWall.transform.position.x - STRIKER_WIDTH / 2);
        this._puckBoundary = new Boundary(_northWall.transform.position.y, _southWall.transform.position.y, _westWall.transform.position.x, _eastWall.transform.position.x);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (_rbPuck.position.y < _centerY)
        {
            if (_isInOpponentsHalf)
            {
                _isInOpponentsHalf = false;
                _offsetXMovement = Random.Range(-1, 1f);
            }

            _movementSpeed = _maxMovementSpeed * Random.Range(0.1f, 0.3f);
            _targetPos = new Vector2(Mathf.Clamp(_rbPuck.position.x + _offsetXMovement, _boundary.left, _boundary.right), _startingPos.y);
        }
        else
        {
            _isInOpponentsHalf = true;

            _movementSpeed = Random.Range(_maxMovementSpeed * 0.4f, _maxMovementSpeed);

            _targetPos = new Vector2(Mathf.Clamp(_rbPuck.position.x, _boundary.left, _boundary.right), Mathf.Clamp(_rbPuck.position.y, _boundary.bottom, _boundary.top));
        }

    }

    void LateUpdate()
    {
        _rb.MovePosition(Vector2.MoveTowards(_rb.position, _targetPos, _movementSpeed * Time.deltaTime));
    }
}
