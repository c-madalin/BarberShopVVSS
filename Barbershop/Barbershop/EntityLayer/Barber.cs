using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.EntityLayer
{
    internal sealed class Barber: User
    {
        public string Specialisation { get; set; }
        public decimal Salary { get; set; }

        public Barber() => Role = Enums.UserRole.Barber;
    }
}
