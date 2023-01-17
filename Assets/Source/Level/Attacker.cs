using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class Attacker : MonoBehaviour, IComponent
    {
        [SerializeField] private int _damage;
        [SerializeField] private LayerMask _attackable;
        [SerializeField] private Vector2 _zone;
        
        private Collider2D[] _hitBuffer = new Collider2D[10];
        private List<Collider2D> _hitBufferList = new List<Collider2D>(10);

        public event Action<IAttackable> Attacked;

        public Vector2 Zone => _zone;
        
        private void FixedUpdate() => 
            Attack();

        public void SwitchOn() => 
            enabled = true;

        public void SwitchOff() => 
            enabled = false;

        private void Attack()
        {
            Vector2 position = transform.position;
            float angle = transform.rotation.eulerAngles.z;

            int collisionCount = Physics2D.OverlapBoxNonAlloc(position, _zone, angle, _hitBuffer, _attackable);
            if (collisionCount > 0)
            {
                _hitBufferList.Clear();
                
                for (int i = 0; i < collisionCount; i++)
                {
                    _hitBufferList.Add(_hitBuffer[i]);
                }
                
                for (int i = 0; i < _hitBufferList.Count; i++)
                {
                    var col = _hitBufferList[i];

                    if (col.transform.TryGetComponent(out IAttackable attackable))
                    {
                        attackable.TakeDamage(_damage);
                        Attacked?.Invoke(attackable);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _zone);
        }
    }
}
