using System;

namespace Source
{
    public class Timer2
    {
        private float _target;
        private float _elapsed;

        private bool _started;
        private bool _ended;
        private bool _stopped;

        public event Action Started;
        public event Action Ended;
        
        public void Update(float deltaTime)
        {
            if(_stopped)
                return;
            
            if (_started)
            {
                _started = false;
                Started?.Invoke();
            }

            _elapsed += deltaTime;

            if (_elapsed >= _target)
            {
                Ended?.Invoke();
            }
        }

        public Timer2 Start()
        {
            _started = true;
            _stopped = false;
            return this;
        }

        public Timer2 Stop()
        {
            _stopped = true;
            return this;
        }

        public Timer2 Restart()
        {
            Reset();
            Start();
            return this;
        }

        public Timer2 Resume()
        {
            _stopped = false;
            return this;
        }

        private void Reset() =>
            _elapsed = 0;
    }
}