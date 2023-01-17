using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Source
{
    public class Stair : RaycastCollision
    {
        [SerializeField] private float _upStairSpeed;
        [SerializeField] private int _upStairDirection;
        [SerializeField] private LayerMask _passengerMask;

        private Dictionary<HorizontalMove, Controller2D> _controllers;
        public float UpStaringSpeed => _upStairSpeed;
        public float UpStarDirection => _upStairDirection;

        private void OnValidate()
        {
            _upStairDirection = Mathf.Clamp(_upStairDirection, -1, 1);
            _upStairDirection = _upStairDirection == 0 ? 1 : _upStairDirection;
        }

        private void FixedUpdate()
        {
            CalculateRays();
        }

        protected override void Start()
        {
            base.Start();

            _controllers = new Dictionary<HorizontalMove, Controller2D>();
        }
    }
}