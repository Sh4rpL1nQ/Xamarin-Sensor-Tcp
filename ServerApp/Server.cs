using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Package = Library.Package;

namespace ServerApp
{
    public class Server
    {
        private Socket serverSocket;

        public ObservableCollection<Player> Players { get; set; }

        private List<Role> roles;

        public Server()
        {
            var roleTypes = Enum.GetValues(typeof(RoleType)).Cast<RoleType>().ToList();
            roles = new List<Role>();

            roleTypes.ForEach(x => roles.Add(new Role() { RoleType = x }));
        }

        public event EventHandler OnPlayerConnected;
        public event EventHandler OnPlayerDisconnected;

        public void Start()
        {
            Players = new ObservableCollection<Player>();

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Package.GetIpAdress()), 11000));

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
        }

        public void ListenThread()
        {
            while (true)
            {
                serverSocket.Listen(0);
                var player = new Player(serverSocket.Accept(), this);
                if (Players.Count < roles.Count)
                {
                    player.SendRegistrationPackage(roles);
                    Players.Add(player);
                    OnPlayerConnected?.Invoke(player, new EventArgs());
                }                
                else
                {
                    var p = new Package(PackageType.ServerFull, "server");
                    p.data.Add(player.Id);
                    player.Socket.Send(p.ToBytes());
                }
            }
        }

        public void DataIn(object data)
        {
            Player player = (Player)data;
            byte[] buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    buffer = new byte[player.Socket.SendBufferSize];
                    readBytes = player.Socket.Receive(buffer);
                    if (readBytes > 0)
                    {
                        Package p = new Package(buffer);
                        DataManager(p);
                    }
                }
                catch (Exception e)
                {
                    OnPlayerDisconnected?.Invoke(player, new EventArgs());
                    var p = new Package(PackageType.Disconnected, player.Id);

                    roles.FirstOrDefault(x => x.RoleType == player.Role).IsVisible = true;
                    p.data.Add(player.Role.ToString());

                    Players.Remove(player);

                    for (int i = 0; i < Players.Count; i++)
                        Players[i].Socket.Send(p.ToBytes());

                    break;
                }
            }
        }

        public void DataManager(Package p)
        {
            switch (p.packetType)
            {
                case PackageType.Selected:
                    var enumValue = (RoleType) Enum.Parse(typeof(RoleType), p.data[0].ToString());
                    foreach (var c in Players)
                    {
                        if (c.Id == p.senderId)
                            c.Role = enumValue;

                        roles.FirstOrDefault(x => x.RoleType == enumValue).IsVisible = false;

                        p.data.Add(roles);
                        c.Socket.Send(p.ToBytes());
                    }
                    break;

                case PackageType.Sensor:
                    var split = p.data[0].ToString().Split('|');
                    var target = Players.FirstOrDefault(x => x.Id == p.senderId);
                    target.X = float.Parse(split[0], CultureInfo.InvariantCulture);
                    target.Y = float.Parse(split[1], CultureInfo.InvariantCulture);
                    target.Z = float.Parse(split[2], CultureInfo.InvariantCulture);
                    break;
            }
        }
    }
}
