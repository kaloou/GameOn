using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameOn.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? nom = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));

        protected void OnAllPropertiesChanged()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

    }
}
