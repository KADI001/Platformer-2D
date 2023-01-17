using UnityEngine;

namespace Source
{
    public interface IGameFactory
    {
        Player CreatePlayer();
        Bullet CreateBullet();
    }
}