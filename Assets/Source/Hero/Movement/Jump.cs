using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(IMoveable))]
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float _height;
        [SerializeField] private float _timeToJumpApex;

        private float _speed;
        private bool _isClimbingStair;
        private IMoveable _controller;
        private Gravity _gravity;
        private UpStair _upStair;

        public bool onGround;
        

        private void Awake()
        {
            _controller = GetComponent<IMoveable>();
            _gravity = GetComponent<Gravity>();
            _upStair = GetComponent<UpStair>();
        }

        private void Start()
        {
            _gravity.SetAcceleration(2 * _height / Mathf.Pow(_timeToJumpApex, 2));
            _speed = Mathf.Abs(_gravity.Acceleration) * _timeToJumpApex +
                     Mathf.Abs(_gravity.Acceleration * Time.deltaTime);
        }

        private void Update()
        {
            if (_controller.Velocity.y < 0) 
                _upStair?.TurnOn();

            if (Input.GetKeyDown(KeyCode.W) && _controller.OnGround)
            {
                _controller.SetVelocity(new Vector2(_controller.Velocity.x, _speed));
                _upStair?.TurnOff();
            }

            onGround = _controller.OnGround;
        }
    }
}