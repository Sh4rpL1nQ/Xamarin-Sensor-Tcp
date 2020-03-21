using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ClientApp
{
    public class ConnectionViewModel : PropertyChangedBase
    {
        private string host;
        private string port;

        public string Host
        {
            get { return host; }
            set { RaisePropertyChanged(ref host, value); }
        }

        public string Port
        {
            get { return port; }
            set { RaisePropertyChanged(ref port, value); }
        }

        public ConnectionViewModel()
        {
            ConnectCommand = new Command(
                execute: () => { ConnectAction(); },
                canExecute: () => { return true; }
                );
        }

        public ICommand ConnectCommand { get; }

        public void ConnectAction()
        {

        }
    }
}
