using System;
using UnityEngine;

namespace Source
{
    public class Health : MonoBehaviour, IAttackable
    {
        [SerializeField] private int _value;

        private int Min = 0;
        private int Max = 100;

        public Action<Transform> Die;

        public int Value => _value;
        
        private void OnValidate()
        {
            _value = _value < Min ? Min : _value;
            _value = _value > Max ? Max : _value;
        }

        private void Update()
        {
            print("Health: " + Value);
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentException($"Damage can't be negative or zero. #{damage}#");

            _value -= damage;
            
            if (_value - damage < Min)
            {
                Die?.Invoke(transform);
                Destroy(gameObject);
            }
        }
    }
}
