using Barbershop.EntityLayer;
using Barbershop.EntityLayer.Enums;
using Barbershop.IntegrationLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Barbershop.RepositoryLayer
{
    public sealed class AppointmentRepository : IAppointmentRepository
    {
        public void Add(Appointment appointment)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.Appointments (CustomerEmail, BarberEmail, AppointmentDate, ServiceType, Status)
                VALUES (@CustomerEmail, @BarberEmail, @Date, @Service, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);

            cmd.Parameters.AddWithValue("@CustomerEmail", appointment.CustomerEmail);
            cmd.Parameters.AddWithValue("@BarberEmail", appointment.BarberEmail);
            cmd.Parameters.AddWithValue("@Date", appointment.AppointmentDate);
            cmd.Parameters.AddWithValue("@Service", appointment.ServiceType);
            cmd.Parameters.AddWithValue("@Status", appointment.Status.ToString());

            var result = cmd.ExecuteScalar();
            appointment.AppointmentID = result != null ? Convert.ToInt32(result) : 0;
        }

        public List<Appointment> GetByCustomerEmail(string customerEmail)
        {
            var list = new List<Appointment>();
            using var conn = DbContext.GetConnection();

            string sql = @"
                SELECT a.AppointmentID, a.AppointmentDate, a.ServiceType, a.Status, a.BarberEmail, a.CustomerEmail,
                       b.FirstName + ' ' + b.LastName as BarberName
                FROM dbo.Appointments a
                JOIN dbo.Users b ON a.BarberEmail = b.Email  -- AICI E CHEIA: Legatura pe Email
                WHERE a.CustomerEmail = @Email
                ORDER BY a.AppointmentDate DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", customerEmail);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public List<Appointment> GetByBarberEmail(string barberEmail)
        {
            var list = new List<Appointment>();
            using var conn = DbContext.GetConnection();

            string sql = @"
                SELECT a.AppointmentID, a.AppointmentDate, a.ServiceType, a.Status, a.BarberEmail, a.CustomerEmail,
                       c.FirstName + ' ' + c.LastName as ClientName
                FROM dbo.Appointments a
                JOIN dbo.Users c ON a.CustomerEmail = c.Email -- Legatura pe Email
                WHERE a.BarberEmail = @Email
                ORDER BY a.AppointmentDate DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", barberEmail);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public void DeleteById(int id)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Appointments WHERE AppointmentID = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private Appointment Map(SqlDataReader reader)
        {
            var appt = new Appointment
            {
                AppointmentID = (int)reader["AppointmentID"],
                CustomerEmail = reader["CustomerEmail"].ToString(),
                BarberEmail = reader["BarberEmail"].ToString(),
                AppointmentDate = (DateTime)reader["AppointmentDate"],
                ServiceType = reader["ServiceType"].ToString(),
                Status = Enum.Parse<AppointmentStatus>(reader["Status"].ToString())
            };

            // Incercam sa citim numele doar daca exista in query (pentru ca JOIN-urile difera)
            try { appt.BarberName = reader["BarberName"].ToString(); } catch { }
            try { appt.ClientName = reader["ClientName"].ToString(); } catch { }

            return appt;
        }
    }
}