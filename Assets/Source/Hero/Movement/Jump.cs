using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Controller2D))]
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float _height;
        [SerializeField] private float _timeToJumpApex;

        private float _speed;
        private Controller2D _controller;
        private Gravity _gravity;
        private UpStair _upStair;

        private bool _hasUpStairComponent;

        private void Awake()
        {
            _controller = GetComponent<Controller2D>();
            _gravity = GetComponent<Gravity>();
            _upStair = GetComponent<UpStair>();
        }

        private void Start()
        {
            _gravity.SetAcceleration(2 * _height / Mathf.Pow(_timeToJumpApex, 2));
            _speed = Mathf.Abs(_gravity.Acceleration) * _timeToJumpApex +
                     Mathf.Abs(_gravity.Acceleration * Time.deltaTime);

            _hasUpStairComponent = _upStair != null;
        }

        private void Update()
        {
            if (_controller.Velocity.y < 0)
            {
                _upStair.TurnOn();
            }

            if (Input.GetKeyDown(KeyCode.Space) && _controller.OnGround)
            {
                _controller.SetVelocity(new Vector2(_controller.Velocity.x, _speed));
                _upStair.TurnOff();
            }
        }
    }
}