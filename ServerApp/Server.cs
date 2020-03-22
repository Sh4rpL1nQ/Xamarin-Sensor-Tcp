using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private List<Player> players;

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
            players = new List<Player>();

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
                if (players.Count < roles.Count)
                {
                    player.SendRegistrationPackage(roles);
                    players.Add(player);
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
                catch (Exception)
                {
                    OnPlayerDisconnected?.Invoke(player, new EventArgs());
                    var p = new Package(PackageType.Disconnected, player.Id);

                    roles.FirstOrDefault(x => x.RoleType == player.Role).IsVisible = true;
                    p.data.Add(player.Role.ToString());

                    players.Remove(player);

                    for (int i = 0; i < players.Count; i++)
                        players[i].Socket.Send(p.ToBytes());

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
                    foreach (var c in players)
                    {
                        if (c.Id == p.senderId)
                            c.Role = enumValue;

                        roles.FirstOrDefault(x => x.RoleType == enumValue).IsVisible = false;

                        p.data.Add(roles);
                        c.Socket.Send(p.ToBytes());
                    }
                    break;
            }
        }
    }
}
