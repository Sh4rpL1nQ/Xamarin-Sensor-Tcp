using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class SelectedRoleEventArgs : EventArgs
    {
        public SelectedRoleEventArgs(RoleType roleType, List<RoleType> serverRoles)
        {
            RoleType = roleType;
            ServerRoles = serverRoles;
        }

        public RoleType RoleType { get; set; }

        public List<RoleType> ServerRoles { get; set; }
    }
}
