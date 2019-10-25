using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BountyCalculator.ApiInteraction
{
    public class ExpirationWrapper<T> : IExpirable, IEquatable<ExpirationWrapper<T>>, IEquatable<T>
    {
        private readonly Timer _timer;
        private bool _enabled;
        private readonly object _lock;

        public ExpirationWrapper(T value, int expirationTimeInMilliseconds)
        {
            ExpirationTime = expirationTimeInMilliseconds;
            _enabled = true;
            _lock = new object();

            _timer = new Timer(TimerExpired, null, expirationTimeInMilliseconds, Timeout.Infinite);
            Value = value;
        }

        private void TimerExpired(object state)
        {
            lock (_lock)
            {
                if (_enabled)
                {
                    Expired?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public T Value { get; private set; }

        public bool IsExpired { get; private set; }

        public int ExpirationTime { get; private set; }

        public event EventHandler<EventArgs> Expired;

        public void SetTimer(int timeInMilliseconds)
        {
            lock (_lock)
            {
                if (_enabled)
                {
                    StopTimer();
                    ExpirationTime = timeInMilliseconds;
                    RestartTimer();
                }
                else
                {
                    ExpirationTime = timeInMilliseconds;
                }
            }
        }

        public void RestartTimer()
        {
            lock (_lock)
            {
                _enabled = true;
                _timer.Change(ExpirationTime, Timeout.Infinite);
            }
        }

        public void StopTimer()
        {
            lock (_lock)
            {
                _enabled = false;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        #region [ IEquatable Implementation ]

        public bool Equals(T other)
        {
            return Value.Equals(other);
        }

        public bool Equals(ExpirationWrapper<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is ExpirationWrapper<T>) return Equals((ExpirationWrapper<T>)obj);
            if (obj is T) return Equals((T)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        #endregion
    }
}
