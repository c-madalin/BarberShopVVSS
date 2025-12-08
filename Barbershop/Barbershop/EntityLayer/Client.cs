using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.EntityLayer
{
    public sealed class Client: User
    {
        public Client() => Role = Enums.UserRole.Client;
    }
}
