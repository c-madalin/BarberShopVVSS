using Barbershop.EntityLayer;
using System.Collections.Generic;

namespace Barbershop.RepositoryLayer
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);
        List<Appointment> GetByCustomerEmail(string email);
        List<Appointment> GetByBarberEmail(string email);
        void DeleteById(int id);
    }
}