using UnityEngine;

namespace Source
{
    public class ResourceAssetsProvider : IAssetsProvider
    {
        private const string Player = "Prefabs/Player";
        private const string Enemy = "Prefabs/Enemy";
        private const string Bullet = "Prefabs/Bullet";

        public Player GetPlayer() => 
            Instantiate<Player>(Player);

        public Enemy GetEnemy() =>
            Instantiate<Enemy>(Enemy);

        public Bullet GetBullet() => 
            Instantiate<Bullet>(Bullet);

        private static T Instantiate<T>(string path) where T : notnull, MonoBehaviour =>
            Instantiate<T>(path, Vector2.zero);

        private static T Instantiate<T>(string path, Vector2 at) where T : notnull, MonoBehaviour =>
            Object.Instantiate(Resources.Load<T>(path), at, Quaternion.identity);
        
    }
}