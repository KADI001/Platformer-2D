namespace Source
{
    public class Game
    {
        private readonly GameStateMachine _gameStateMachine;
        
        public Game(ICoroutineRunner coroutineRunner, IAssetsProvider assetsProvider)
        {
            _gameStateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), ProjectContext.Container, assetsProvider);
        }

        public void Start()
        {
            GameStateMachine.Enter<BootstrapState>();
        }
        
        public GameStateMachine GameStateMachine => _gameStateMachine;
    }
}