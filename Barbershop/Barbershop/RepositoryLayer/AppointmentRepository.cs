using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Barbershop.RepositoryLayer;
using Microsoft.Data.SqlClient;
using System.Data;

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;

namespace Barbershop.RepositoryLayer
{
    public class AppointmentRepository : IUserRepository<Appointments>
    {
        public void Add(Appointments appointment)
        {
            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Appointments (CustomerEmail, BarberEmail, AppointmentDate, ServiceType)
VALUES (@CustomerEmail, @BarberEmail, @AppointmentDate, @ServiceType);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn)
            {
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@CustomerEmail", appointment.CustomerEmail ?? string.Empty);
            cmd.Parameters.AddWithValue("@BarberEmail", appointment.BarberEmail ?? string.Empty);
            cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
            cmd.Parameters.AddWithValue("@ServiceType", appointment.ServiceType ?? string.Empty);

            var result = cmd.ExecuteScalar();
            appointment.AppointmentID = result != null ? Convert.ToInt32(result) : 0;
        }

        // Returns the most recent appointment for the given customer email.
        public Appointments GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
SELECT TOP 1 AppointmentID, CustomerEmail, BarberEmail, AppointmentDate, ServiceType
FROM dbo.Appointments
WHERE CustomerEmail = @Email
ORDER BY AppointmentDate DESC;", conn)
            {
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return Map(reader);
            }

            return null;
        }

        // Additional helpers used by domain/service
        public List<Appointments> GetByCustomerEmail(string customerEmail)
        {
            var list = new List<Appointments>();
            if (string.IsNullOrWhiteSpace(customerEmail)) return list;

            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
SELECT AppointmentID, CustomerEmail, BarberEmail, AppointmentDate, ServiceType
FROM dbo.Appointments
WHERE CustomerEmail = @CustomerEmail
ORDER BY AppointmentDate;", conn)
            {
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@CustomerEmail", customerEmail);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public List<Appointments> GetByBarberEmail(string barberEmail)
        {
            var list = new List<Appointments>();
            if (string.IsNullOrWhiteSpace(barberEmail)) return list;

            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand(@"
SELECT AppointmentID, CustomerEmail, BarberEmail, AppointmentDate, ServiceType
FROM dbo.Appointments
WHERE BarberEmail = @BarberEmail
ORDER BY AppointmentDate;", conn)
            {
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@BarberEmail", barberEmail);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public void DeleteById(int appointmentId)
        {
            if (appointmentId <= 0) return;

            using var conn = DbContext.GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Appointments WHERE AppointmentID = @AppointmentID;", conn)
            {
                CommandType = CommandType.Text
            };

            cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
            cmd.ExecuteNonQuery();
        }

        private Appointments Map(SqlDataReader reader)
        {
            return new Appointments
            {
                AppointmentID = reader["AppointmentID"] != DBNull.Value ? Convert.ToInt32(reader["AppointmentID"]) : 0,
                CustomerEmail = reader["CustomerEmail"]?.ToString(),
                BarberEmail = reader["BarberEmail"]?.ToString(),
                AppointmentDate = reader["AppointmentDate"] != DBNull.Value ? Convert.ToDateTime(reader["AppointmentDate"]) : DateTime.MinValue,
                ServiceType = reader["ServiceType"]?.ToString()
            };
        }
    }
}