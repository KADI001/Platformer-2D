using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(IMoveable))]
    public class Walk : MonoBehaviour, IComponent
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _bounceOffStairForce;

        private int _input;
        private IMoveable _controller2D;
        private UpStair _upStair;

        private void Awake()
        {
            _controller2D = GetComponent<IMoveable>();
            _upStair = GetComponent<UpStair>();
        }

        private void Update() =>
            _input = GetMoveDirection();

        private void FixedUpdate()
        {
            _controller2D.SetVelocity(new Vector2(_input * _speed, _controller2D.Velocity.y));  

            if (_upStair.isClimbing && Input.GetKeyDown(KeyCode.Space))
            {
                _controller2D.SetVelocity(Vector2.left * _bounceOffStairForce);
            }
        }

        public static int GetMoveDirection()
        {
            if (Input.GetKey(KeyCode.D))
                return 1;

            if (Input.GetKey(KeyCode.A))
                return -1;

            return 0;
        }

        public void SwitchOn() =>
            enabled = true;

        public void SwitchOff() =>
            enabled = false;
    }
}