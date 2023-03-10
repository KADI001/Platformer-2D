using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(IMoveable))]
    public class Gravity : MonoBehaviour
    {
        [SerializeField] private float _acceleration;
        private IMoveable _controller;

        public float Acceleration => _acceleration;

        private void Awake()
        {
            _controller = GetComponent<IMoveable>();
        }

        private void FixedUpdate()
        {
            if (!_controller.OnGround)
            {
                _controller.AddVelocity(Vector2.down * (_acceleration * Time.deltaTime));
            }
        }

        public void SetAcceleration(float acceleration)
        {
            _acceleration = acceleration;
        }
    }
}