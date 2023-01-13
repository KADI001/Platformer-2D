using System;
using UnityEngine;

namespace Source
{
    public class Bullet : MonoBehaviour
    {
        private float _speed;
        private int _direction;

        private void Awake()
        {
            _direction = 1;
        }

        private void FixedUpdate()
        {
            transform.position += Vector3.right * (_speed * _direction * Time.deltaTime);
        }

        public void Init(Vector2 at, int to, float speed)
        {
            _speed = speed;
            _direction = to;
            transform.position = at;
        }
    }
}