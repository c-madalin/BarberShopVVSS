using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.EntityLayer.Enums
{
    public enum UserRole
    {
        Client,
        Barber,
        Admin
    }
    public enum AppointmentStatus
    {
        Pending,   
        Confirmed, 
        Completed, 
        Canceled   
    }
}
