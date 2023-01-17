using UnityEngine;

namespace Source
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private SOAssetsProvider _assetsProvider;
        
        private Game _game;

        private void Awake()
        {
            _game = new Game(this, new ResourceAssetsProvider());
            _game.Start();
        
            DontDestroyOnLoad(this);
        }
    }
}