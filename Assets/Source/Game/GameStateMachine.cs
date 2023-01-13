using System;
using System.Collections.Generic;
using UnityEngine;

namespace Source
{
    public class GameStateMachine 
    {
        private readonly Dictionary<Type, IExiteableState> _states;
        private IExiteableState _activeState;

        public GameStateMachine(SceneLoader sceneLoader)
        {
            _states = new Dictionary<Type, IExiteableState>()
            {
                [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader),
                [typeof(LoadGameState)] = new LoadGameState(this, sceneLoader),
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload arg) where TState : class, IPayloadedState<TPayload>
        {
            IPayloadedState<TPayload> state = ChangeState<TState>();
            state.Enter(arg);
        }

        private TState ChangeState<TState>() where TState : class, IExiteableState
        {
            _activeState?.Exit();
            TState state = GetState<TState>();
            _activeState = state;
            return state;
        }

        private TState GetState<TState>() where TState : class, IExiteableState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}