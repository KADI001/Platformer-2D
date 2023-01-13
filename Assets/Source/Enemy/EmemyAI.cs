using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Source
{
    public class EmemyAI : RaycastCollision
    {
        [SerializeField] private float _speed;
        
        private SpriteRenderer _sprite;
        private int _moveDirection;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        protected override void Start()
        {
            base.Start();

            _steps = 5;
            _moveDirection = 1;
        }

        private void FixedUpdate()
        {
            CalculateRays();

            float deltaX  = _speed * _moveDirection * Time.deltaTime;
            
            Move(ref deltaX);

            transform.position += Vector3.right * deltaX;
        }

        private void Move(ref float deltaX)
        {
            RayRange rayRange = _moveDirection == 1 ? _right : _left;

            bool collided = false;
            float maxDistance = 0;
            
            Physics2DEx.RaycastWithAction(rayRange, deltaX + _shell, _collisionMask, _steps, hit =>
            {
                collided = true;
                maxDistance = hit.distance;
            });

            if (collided)
            {
                deltaX = (maxDistance - _shell) * _moveDirection;
                _moveDirection *= -1;
                Flip();
            }
        }
        
        private void Flip() => 
            _sprite.flipX = _moveDirection == -1;
    }

}