using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.ServiceLayer
{
    public interface IAppointmentService
    {
        public List<Appointment> GetHistoryClient(string email);
        public List<Appointment> GetHistoryBarber(string email);
        public void Cancel(int id);
    }
}
