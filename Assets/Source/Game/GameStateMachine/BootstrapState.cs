using System;

namespace Source
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IAssetsProvider _assetsProvider;
        private readonly ProjectContext _projectContext;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, IAssetsProvider assetsProvider, ProjectContext projectContext)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _assetsProvider = assetsProvider;
            _projectContext = projectContext;

            SetUpServices();
        }

        public void Enter()
        {
            _sceneLoader.Load(Initial, EnterLoadGameState);
        }

        public void Exit()
        {
        }

        private void SetUpServices()
        {
            _projectContext.RegisterAsSingle<IAssetsProvider>(_assetsProvider);
            _projectContext.RegisterAsSingle<IGameFactory>(new GameFactory(_projectContext.GetSingle<IAssetsProvider>()));
        }

        private void EnterLoadGameState() => 
            _gameStateMachine.Enter<LoadGameState, string>("Level1");
    }
}