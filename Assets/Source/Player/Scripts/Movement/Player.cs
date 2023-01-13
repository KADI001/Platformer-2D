using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Controller2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private int _input;
        private Controller2D _controller2D;

        private void Awake()
        {
            _controller2D = GetComponent<Controller2D>();
        }

        private void Start()
        {
            //_gravity = -2 * _jumpHeight / Mathf.Pow(_timeToJumpApex, 2);
            //_jumpVelocity = Mathf.Abs(_gravity) * _timeToJumpApex + Mathf.Abs(_gravity * Time.deltaTime);
        }
        
        private bool temp;

        private void Update()
        {
            _input = GetMoveDirection();

           
            if (Input.GetKeyDown(KeyCode.F))
            {
                temp = !temp;
            }

            if (temp)
            {
                _input = 1;
            }
            
        }

        private void FixedUpdate()
        {
            _controller2D.SetVelocity(new Vector2(_input * _speed, _controller2D.Velocity.y));
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
    }
}