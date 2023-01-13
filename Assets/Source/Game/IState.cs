namespace Source
{
    public interface IState : IExiteableState
    {
        void Enter();
    }

    public interface IPayloadedState<T> : IExiteableState
    {
        void Enter(T arg);
    }

    public interface IExiteableState
    {
        void Exit();
    }
}