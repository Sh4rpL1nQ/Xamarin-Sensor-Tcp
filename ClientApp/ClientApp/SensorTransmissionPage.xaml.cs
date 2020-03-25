using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ClientApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SensorTransmissionPage : ContentPage
    {
        public SensorTransmissionPage()
        {
            InitializeComponent();
        }

        public SensorTransmissionPage(Client client) : this()
        {
            BindingContext = new SensorTransmissionViewModel(client);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}