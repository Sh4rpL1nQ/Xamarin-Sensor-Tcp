using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ServerApp
{
    public class MainViewModel : PropertyChangedBase
    {
        public Server Server { get; set; }

        public ObservableCollection<Player> Players { get; set; }

        public ObservableCollection<string> BackLog { get; set; }

        public MainViewModel()
        {
            Server = new Server();
            Server.OnPlayerConnected += Server_OnPlayerConnected;
            Server.OnPlayerDisconnected += Server_OnPlayerDisconnected;

            BackLog = new ObservableCollection<string>();

            Players = new ObservableCollection<Player>();

            StartServerCommand = new ActionCommand(StartServerAction);
        }

        private void Server_OnPlayerDisconnected(object sender, EventArgs e)
        {
            var player = sender as Player;
            Players.Add(player);
            AddLog("Player disconnected: " + (sender as Player).Id);
        }

        private void Server_OnPlayerConnected(object sender, EventArgs e)
        {
            var player = sender as Player;
            Players.Remove(player);
            AddLog("Player connected: " + (sender as Player).Id);
        }

        public ICommand StartServerCommand { get; }

        public void StartServerAction(object sender)
        {
            Server.Start();
            BackLog.Add("Starting Server on: " + Package.GetIpAdress());
        }

        private void AddLog(string message)
        {
            DispatcherObject.BeginInvoke(new Action(() => BackLog.Add(message)));
        }
    }
}
