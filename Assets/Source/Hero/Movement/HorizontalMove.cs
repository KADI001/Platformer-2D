using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Controller2D))]
    public class HorizontalMove : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private int _input;
        private Controller2D _controller2D;

        private void Awake() => 
            _controller2D = GetComponent<Controller2D>();

        private void Update() => 
            _input = GetMoveDirection();

        private void FixedUpdate() => 
            _controller2D.SetVelocity(new Vector2(_input * _speed, _controller2D.Velocity.y));

        public static int GetMoveDirection()
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