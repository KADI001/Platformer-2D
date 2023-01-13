using System;
using UnityEngine;

namespace Source
{
    public class Pistol : MonoBehaviour, IWeapon
    {
        [SerializeField] private Bullet _bulletPrefab;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            Get(transform.position, 1, 2);
        }

        public Bullet Get(Vector2 at, int to, float speed)
        {
            Bullet bullet = Instantiate(_bulletPrefab);
            bullet.Init(at, to, speed);
            return bullet;
        }
    }
}