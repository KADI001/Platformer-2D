using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Collider2D))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;
        
        private int _direction;
        private int _damage;
        private float _elapsedTime;

        public void Init(Vector2 from, int to, int damage)
        {
            transform.position = from;
            _direction = to;
            _damage = damage;
        }

        private void FixedUpdate()
        {
            if(_elapsedTime >= _lifeTime)
                Destroy(gameObject);

            _elapsedTime += Time.deltaTime;
            transform.position += Vector3.right * (_speed * _direction * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.transform.TryGetComponent(out Enemy enemy))
            {
                if (enemy.TryGetComponent(out IAttackable attackable))
                    attackable.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }
    }
}