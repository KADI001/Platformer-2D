namespace Source
{
    public class Game
    {
        private readonly GameStateMachine _gameStateMachine;
        
        public Game(ICoroutineRunner coroutineRunner)
        {
            _gameStateMachine = new GameStateMachine(new SceneLoader(coroutineRunner));
        }

        public void Start()
        {
            GameStateMachine.Enter<BootstrapState>();
        }
        
        public GameStateMachine GameStateMachine => _gameStateMachine;
    }
}