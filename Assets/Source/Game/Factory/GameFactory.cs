using UnityEngine;

namespace Source
{
    public class GameFactory : IGameFactory
    {
        private readonly IAssetsProvider _assetsProvider;

        public GameFactory(IAssetsProvider assetsProvider)
        {
            _assetsProvider = assetsProvider;
        }

        public Player CreatePlayer() =>
            _assetsProvider.GetPlayer();

        public Bullet CreateBullet() => 
            _assetsProvider.GetBullet();
    }
}