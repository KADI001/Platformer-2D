using System;

namespace Source
{
    public class Timer
    {
        public readonly float EndTime;

        public event Action Finished;
        public event Action Ticked;

        public Timer(float endTime) => 
            EndTime = endTime;

        public bool IsRunning { get; private set; }
        public float AccumulatedTime { get; private set; }
        public bool IsStopped => IsRunning == false && AccumulatedTime > 0;

        public void Update(float deltaTime)
        {
            if(IsRunning == false)
                return;

            AccumulatedTime += deltaTime;

            if (AccumulatedTime >= EndTime)
            {
                IsRunning = false;
                Finished?.Invoke();
            }

            Ticked?.Invoke();
        }

        public Timer Start(Action onFinished = null)
        {
            AccumulatedTime = 0f;
            IsRunning = true;
            
            if(onFinished != null)
                Finished += onFinished;
            return this;
        }

        public Timer Reset()
        {
            AccumulatedTime = 0f;
            IsRunning = false;
            
            return this;
        }

        public Timer Stop()
        {
            IsRunning = false;
            return this;
        }
    }
}