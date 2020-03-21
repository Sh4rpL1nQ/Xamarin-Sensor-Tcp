using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace ServerApp
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Dispatcher DispatcherObject { get; protected set; }

        public PropertyChangedBase()
        {
            DispatcherObject = Dispatcher.CurrentDispatcher;
        }

        public bool RaisePropertyChanged<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val))
                return false;

            member = val;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
