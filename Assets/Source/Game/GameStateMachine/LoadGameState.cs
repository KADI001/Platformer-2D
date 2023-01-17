using UnityEngine;

namespace Source
{
    public class LoadGameState : IPayloadedState<string>
    {
        public const string InitialPointTag = "InitialPoint";
        
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly IGameFactory _gameFactory;

        public LoadGameState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, IGameFactory gameFactory)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _gameFactory = gameFactory;
        }
        
        
        public void Enter(string sceneName)
        {
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        public void Exit()
        {
        }

        private void OnLoaded()
        {
            Vector2 point = GameObject.FindWithTag(InitialPointTag).transform.position;
            _gameFactory.CreatePlayer().
                With(p => p.transform.position = point);
        }
    }
}