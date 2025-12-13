using Barbershop.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.DomainLayer
{
    public interface IAppointmentDomain
    {
        public void Create(Appointment appointment);
        public List<Appointment> GetByCustomerEmail(string email);
        public List<Appointment> GetByBarberEmail(string email);
        public void Cancel(int id);
    }
}
