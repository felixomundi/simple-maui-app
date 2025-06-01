using System;
namespace users.Services
{

    public class InactivityTimer
    {
        private readonly TimeSpan _timeout;
        private Timer _timer;

        public event Action? OnTimeout;

        public InactivityTimer(TimeSpan timeout)
        {
            _timeout = timeout;
            _timer = new Timer(OnTimedEvent, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start() => Reset();

        public void Reset()
        {
            _timer?.Change(_timeout, Timeout.InfiniteTimeSpan);
        }

        private void OnTimedEvent(object? state)
        {
            OnTimeout?.Invoke();
        }
    }

}
