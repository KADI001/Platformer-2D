using System;

namespace Source
{
    public class BootstrapState : IState
    {
        private const string Initial = "Initial";
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter()
        {
            SetUpServices();
            _sceneLoader.Load(Initial, EnterLoadGameState);
        }

        public void Exit()
        {
        }

        private void SetUpServices()
        {
        }

        private void EnterLoadGameState() => 
            _gameStateMachine.Enter<LoadGameState, string>("Level1");
    }
}