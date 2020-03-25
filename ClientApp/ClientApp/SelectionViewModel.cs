using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace ClientApp
{
    public class SelectionViewModel : PropertyChangedBase
    {
        private Role selection;

        public SelectionViewModel()
        {
            SelectionCommand = new Command(async (sender) =>
            {
                Client.Send(PackageType.Selected, (sender as Role).RoleType.ToString());
                await Application.Current.MainPage.Navigation.PushAsync(new SensorTransmissionPage(Client));
            });
        }

        public SelectionViewModel(Client client) :this()
        {
            Client = client;
        }

        public Client Client { get; set; }

        public ICommand SelectionCommand { get; set; }
    }
}
