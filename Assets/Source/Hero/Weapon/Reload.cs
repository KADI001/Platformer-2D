using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(IWeapon))]
    public class Reload : MonoBehaviour, IComponent
    {
        [SerializeField] private int _amountBullets;
        [SerializeField] private float _reloadInSeconds;

        private IWeapon _weapon;
        private DelayBetweenShots _delayBetweenShots;
        private Timer _timerForReload;
        private int _bullets;
        
        private void Awake()
        {
            _weapon = GetComponent<IWeapon>();
            _delayBetweenShots = GetComponent<DelayBetweenShots>();
            _timerForReload = new Timer(_reloadInSeconds);
            _bullets = _amountBullets;
        }

        private void Update() => 
            _timerForReload.Update(Time.deltaTime); 

        private void OnEnable() => 
            _weapon.Shot += OnShot;

        private void OnDisable() => 
            _weapon.Shot -= OnShot;

        private void OnShot()
        {
            if(!enabled)
                return;
            
            _bullets--;

            if (_bullets <= 0)
            {
                _weapon.SwitchOff();
                
                _delayBetweenShots?.SwitchOff();
                
                _timerForReload.Start(() =>
                {
                    _bullets = _amountBullets;
                    _weapon.SwitchOn();
                    _delayBetweenShots?.SwitchOn();
                });
            }
        }
        
        public void SwitchOff() => 
            enabled = false;

        public void SwitchOn() => 
            enabled = true;
    }
}