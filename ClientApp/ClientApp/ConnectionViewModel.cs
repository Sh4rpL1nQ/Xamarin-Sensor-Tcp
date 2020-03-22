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
        private string host = "192.168.178.22";
        private string port = "11000";
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
            ConnectCommand = new Command (async () => { await ConnectAction(); });

        }

        public ICommand ConnectCommand { get; }

        public async Task ConnectAction()
        {
            if (int.TryParse(port, out int intPort) && !string.IsNullOrEmpty(host))
            {
                client = new Client(host, intPort);

                client.ServerFull += delegate
                {
                    DependencyService.Get<IMessage>().LongAlert("Could not connect to the server: Server is already full");
                    return;
                };

                if (!client.Initiate())
                {
                    DependencyService.Get<IMessage>().LongAlert("Could not connect to the server: Wrong host or wrong port");
                    return;
                }

                await Application.Current.MainPage.Navigation.PushAsync(new SelectionPage(client));
            }  
        }
    }
}
