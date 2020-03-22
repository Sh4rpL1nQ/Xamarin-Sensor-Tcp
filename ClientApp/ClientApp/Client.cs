using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Client
    {
        public Socket Master { get; set; }
        public string Id { get; set; }

        public ObservableCollection<Role> Roles { get; set; }

        public Client(string host, int port)
        {
            Host = host;

            Port = port;

            Roles = new ObservableCollection<Role>();
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool Initiate()
        {
            IPAddress.TryParse(Host, out IPAddress adress);
            var endPoint = new IPEndPoint(adress, Port);
            Master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IAsyncResult result = Master.BeginConnect(endPoint, null, null);

                bool success = result.AsyncWaitHandle.WaitOne(2000, true);

                if (Master.Connected)
                    Master.EndConnect(result);
                else
                {
                    Master.Close();
                    throw new ApplicationException("Failed to connect server.");
                }
            }
            catch (Exception e)
            {
                return false;
            }

            Thread thread = new Thread(DataIn);
            thread.Start();

            return true;
        }

        public event EventHandler ServerFull;

        public void Send(PackageType p, string message)
        {
            var pack = new Package(p, Id);
            pack.data.Add(message);

            Master.Send(pack.ToBytes());
        }

        public async void DataIn()
        {
            byte[] buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    buffer = new byte[Master.SendBufferSize];
                    readBytes = Master.Receive(buffer);
                    if (readBytes > 0)
                    {
                        Package p = new Package(buffer);
                        await DataManager(p);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Server is offline");
                    return;
                }

            }
        }

        public async Task DataManager(Package p)
        {
            switch (p.packetType)
            {
                case PackageType.Connected:                    
                    await Task.Run(() =>
                    {
                        Id = p.data[0].ToString();
                        foreach (var r in (List<Role>)p.data[1])
                            Roles.Add(r);
                    });
                    break;
                case PackageType.Selected:
                    await Task.Run(() =>
                    {
                        Roles.FirstOrDefault(x => x.RoleType == (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString())).IsVisible = false;
                    });                    
                    break;

                case PackageType.Disconnected:
                    Roles.FirstOrDefault(x => x.RoleType == (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString())).IsVisible = true;
                    break;

                case PackageType.ServerFull:
                    ServerFull?.Invoke(p.data[0], new EventArgs());
                    break;
            }
        }
    }
}