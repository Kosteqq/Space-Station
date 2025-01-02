using System;

namespace SpaceStation.Core
{
    public class ObservableProperty<T>
    {
        private T _currentValue;
        
        public T Value
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnValueChange?.Invoke(value);
            }
        }

        public event Action<T> OnValueChange;

        public ObservableProperty()
        {
            Value = default;
        }

        public ObservableProperty(T p_default)
        {
            Value = p_default;
        }
    }
}