using UnityEditor;
using UnityEngine;

namespace Source
{
    [CreateAssetMenu(menuName = "Prefab Set")]
    public class SOAssetsProvider : ScriptableObject, IAssetsProvider
    {
        [Header("Prefabs")]
        [SerializeField] private Player _player;
        [SerializeField] private Enemy _enemy;
        [SerializeField] private Bullet _bullet;
        
        public Enemy GetEnemy() =>
            Instantiate(_enemy);
        
        public Player GetPlayer() =>
            Instantiate(_player);
        
        public Bullet GetBullet() => 
            Instantiate(_bullet);
    }
}