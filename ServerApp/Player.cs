using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp
{
    public class Player : PropertyChangedBase
    {
        private Thread thread;
        private float x, y, z;

        public Player(Socket socket, Action<object> server)
        {
            Socket = socket;

            Id = Guid.NewGuid().ToString();
            thread = new Thread(server.Invoke);
            thread.Start(this);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public RoleType Role { get; set; }

        public Socket Socket { get; }

        public float X
        {
            get { return x; }
            set { RaisePropertyChanged(ref x, value); }
        }

        public float Y
        {
            get { return y; }
            set { RaisePropertyChanged(ref y, value); }
        }

        public float Z
        {
            get { return z; }
            set { RaisePropertyChanged(ref z, value); }
        }

        public void SendRegistrationPackage(List<Role> initRoles)
        {
            var p = new Package(PackageType.Connected, "server");
            p.data.Add(Id);
            p.data.Add(initRoles);
            Socket.Send(p.ToBytes());
        }
    }
}
