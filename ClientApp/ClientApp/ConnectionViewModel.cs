using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ClientApp
{
    public class ConnectionViewModel : PropertyChangedBase
    {
        private string host;
        private string port;
        private Client client;

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
            ConnectCommand = new Command (async () => {
                await ConnectAction();
                await Application.Current.MainPage.Navigation.PushAsync(new SelectionPage(client));
            });
        }

        public ICommand ConnectCommand { get; }

        public async Task ConnectAction()
        {
            client = new Client();
        }
    }
}
