using UnityEngine;

namespace Source
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private Game _game;

        private void Awake()
        {
            _game = new Game(this);
            _game.Start();
        
            DontDestroyOnLoad(this);
        }
    }
}