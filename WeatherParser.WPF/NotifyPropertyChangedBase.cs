using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WeatherParser.WPF
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        protected void OnPropertyChanged<T>(T newValue, ref T refProp, [CallerMemberName] string caller = "")
        {
            if (EqualityComparer<T>.Default.Equals(newValue, refProp))
                return;

            refProp = newValue;
            OnPropertyChanged(caller);
        }
    }
}
