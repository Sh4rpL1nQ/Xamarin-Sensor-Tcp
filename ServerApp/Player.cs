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
    public class Player
    {
        private Thread thread;        

        public Player(Socket socket, Server server)
        {
            Socket = socket;

            Id = Guid.NewGuid().ToString();
            thread = new Thread(server.DataIn);
            thread.Start(this);
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public RoleType Role { get; set; }

        public Socket Socket { get; }

        public void SendRegistrationPackage(List<Role> initRoles)
        {
            var p = new Package(PackageType.Connected, "server");
            p.data.Add(Id);
            p.data.Add(initRoles);
            Socket.Send(p.ToBytes());
        }
    }
}
