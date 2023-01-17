using System;
using UnityEngine;

namespace Source
{
    public class Pistol : MonoBehaviour, IWeapon, IComponent
    {
        [SerializeField] private int _damage;
        [SerializeField] private Transform _shootPoint;

        private IGameFactory _gameFactory;

        public event Action Shot;
        
        private int ShootDirection => _shootPoint.forward.x >= 0 ? 1 : -1;

        private void Awake() => 
            _gameFactory = ProjectContext.Container.GetSingle<IGameFactory>();

        public void Shoot(int direction)
        {
            if (CanShoot())
            {
                _gameFactory.CreateBullet().With(bullet => bullet.Init(_shootPoint.position, ShootDirection, _damage));
                Shot?.Invoke();
            }
        }

        public void SwitchOff() => 
            enabled = false;

        public void SwitchOn() => 
            enabled = true;

        private bool CanShoot() => 
            enabled;
    }
}