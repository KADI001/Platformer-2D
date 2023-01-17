using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Attacker))]
    public class AttackCooldown : MonoBehaviour, IComponent
    {
        [SerializeField] private float _seconds;

        private Attacker _attacker;
        private Timer _timer;

        private void Awake()
        {
            _attacker = GetComponent<Attacker>();
            _timer = new Timer(_seconds);
        }

        private void Update() => 
            _timer.Update(Time.deltaTime);

        private void OnEnable() => 
            _attacker.Attacked += OnAttack;

        private void OnDisable() => 
            _attacker.Attacked -= OnAttack;

        public void SwitchOn() => 
            enabled = true;

        public void SwitchOff() => 
            enabled = false;

        private void OnAttack(IAttackable attackable)
        {
            _attacker.SwitchOff();
            _timer.Start(() => _attacker.SwitchOn());
        }
    }
}