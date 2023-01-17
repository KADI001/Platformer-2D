using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(Attacker))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SyncSizeAttackZoneWithBoxCollider2D : MonoBehaviour, IComponent
    {
        private BoxCollider2D _boxCollider2D;
        private Attacker _attacker;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _attacker = GetComponent<Attacker>();
            Sync();
        }

        private void Update() =>
            Sync();

        public void SwitchOn() => 
            enabled = true;

        public void SwitchOff() => 
            enabled = false;

        private void Sync() => 
            _boxCollider2D.size = _attacker.Zone;
    }
}