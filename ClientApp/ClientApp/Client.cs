using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApp
{
    public class Client
    {
        public Socket Master { get; set; }
        public string Id { get; set; }

        public Client(string host, int port)
        {
            Host = host;

            Port = port;
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

        public event EventHandler SelectionChanged;
        public event EventHandler InitialExecute;
        public event EventHandler ServerFull;

        public void Send(PackageType p, string message)
        {
            var pack = new Package(p, Id);
            pack.data.Add(message);

            Master.Send(pack.ToBytes());
        }

        public void DataIn()
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
                        DataManager(p);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Server is offline");
                    return;
                }

            }
        }

        public void DataManager(Package p)
        {
            switch (p.packetType)
            {
                case PackageType.Connected:
                    Id = p.data[0].ToString();
                    var roles = (bool[])p.data[1];
                    var setup = Enum.GetValues(typeof(RoleType)).Cast<RoleType>().FirstOrDefault(x => roles[(int)x] == true);
                    InitialExecute?.Invoke(this, new SelectedRoleEventArgs(setup, (List<RoleType>)p.data[1]));
                    break;

                case PackageType.Selected:
                    SelectionChanged?.Invoke(this, new SelectedRoleEventArgs((RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString()), (List<RoleType>)p.data[1]));
                    break;

                case PackageType.Disconnected:
                    SelectionChanged?.Invoke(this, new SelectedRoleEventArgs((RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString()), (List<RoleType>)p.data[1]));
                    break;

                case PackageType.ServerFull:
                    ServerFull?.Invoke(p.data[0], new EventArgs());
                    break;
            }
        }
    }
}
