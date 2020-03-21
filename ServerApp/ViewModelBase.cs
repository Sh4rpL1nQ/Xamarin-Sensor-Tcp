using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ServerApp
{
    public class ViewModelBase : PropertyChangedBase
    {
        public virtual Dispatcher DispatcherObject { get; protected set; }

        public ViewModelBase()
        {
            DispatcherObject = Dispatcher.CurrentDispatcher;
        }
    }
}
