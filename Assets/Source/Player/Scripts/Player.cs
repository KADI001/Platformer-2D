using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game{

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float _moveVelocity;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _timeToJumpApex;
    
    private int _input;
    private Vector2 _velocity;
    private float _jumpVelocity;
    private float _gravity;
    private Controller2D _controller2D;

    private bool temp;
    private bool _isUpStaring;
    private bool _pressJump;

    public float InputX => _input;
    public Vector2 Velocity => _velocity;
    public bool OnGround => _controller2D.OnGround;
    
    private void Awake()
    {
        _controller2D = GetComponent<Controller2D>();
    }

    private void Start()
    {
        _gravity = -2 * _jumpHeight / Mathf.Pow(_timeToJumpApex, 2);
        _jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex + Mathf.Abs(_gravity * Time.deltaTime);
    }

    private void Update()
    {
        _input = GetMoveDirection();
        _pressJump = false;

        if (Input.GetKeyDown(KeyCode.F))
        {
            temp = !temp;
        }
        
        if (Input.GetKeyDown(KeyCode.W) && _controller2D.OnGround )
        {
            _pressJump = true;
        }
  
        if (_pressJump)
        {
            _velocity.y = _jumpVelocity;
        }
    }

    private void FixedUpdate()
    {
        _velocity.x = _input * _moveVelocity;
        _velocity.y += _gravity * Time.deltaTime;

        if (temp)
        {
            _velocity.x = 5;
        }
        
        Vector2 deltaPosition = _velocity * Time.deltaTime;
        
        _controller2D.Move(deltaPosition);
        
        if (_controller2D.OnGround || _controller2D.DescendingExcessiveSlope)
        {
            _velocity.y = _velocity.y < 0 ? 0 : _velocity.y;
        }

        if (_controller2D.Above)
        {
            _velocity.y = _velocity.y > 0 ? 0 : _velocity.y;
        }
    }

    public int GetMoveDirection()
    {
        if (Input.GetKey(KeyCode.D))
        {
            return 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            return -1;
        }

        return 0;
    }

    public void SetIsUpStaring(bool isUpStaring)
    {
        _isUpStaring = isUpStaring;
    }
}

}