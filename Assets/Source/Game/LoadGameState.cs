using UnityEngine;

namespace Source
{
    public class LoadGameState : IPayloadedState<string>
    {
        private const string InitialPointTag = "InitialPoint";
        private readonly GameStateMachine _gameStateMachine;
        private readonly SceneLoader _sceneLoader;

        public LoadGameState(GameStateMachine gameStateMachine, SceneLoader sceneLoader)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
        }

        public void Enter(string sceneName)
        {
            _sceneLoader.Load(sceneName, OnLoaded);
        }

        private void OnLoaded()
        {
            GameObject playerInitialPoint = GameObject.FindWithTag(InitialPointTag);
            GameObject player = Instantiate("Prefabs/Player", playerInitialPoint.transform.position);
        }

        public void Exit()
        {
        }

        private GameObject Instantiate(string path)
        {
            return Instantiate(path, Vector2.zero);
        }
        
        private GameObject Instantiate(string path, Vector2 at)
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return Object.Instantiate(prefab, at, Quaternion.identity);
        }
    }
}