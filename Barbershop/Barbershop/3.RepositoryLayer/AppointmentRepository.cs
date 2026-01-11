using Barbershop.EntityLayer;
using Barbershop.EntityLayer.Enums;
using Barbershop.IntegrationLayer;
using Barbershop.Utils.Logging; // Necesar pentru AppLogger
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    public sealed class AppointmentRepository : IAppointmentRepository
    {
        public async Task AddAsync(Appointment appointment)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertAppointment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@CustomerEmail", appointment.CustomerEmail);
                        cmd.Parameters.AddWithValue("@BarberEmail", appointment.BarberEmail);
                        cmd.Parameters.AddWithValue("@AppointmentDate", appointment.AppointmentDate);
                        cmd.Parameters.AddWithValue("@ServiceType", appointment.ServiceType);
                        cmd.Parameters.AddWithValue("@Status", (int)appointment.Status);

                        await conn.OpenAsync();
                        var result = await cmd.ExecuteScalarAsync();
                        appointment.AppointmentID = result != null ? Convert.ToInt32(result) : 0;
                    }
                }
                AppLogger.Info($"Appointment created in DB for {appointment.CustomerEmail} with {appointment.BarberEmail}");
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error adding Appointment: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Appointment>> GetByCustomerEmailAsync(string customerEmail)
        {
            var list = new List<Appointment>();
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_GetAppointmentsByClient", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", customerEmail);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(Map(reader));
                            }
                        }
                    }
                }
                return list;
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error fetching client history ({customerEmail}): {ex.Message}");
                throw;
            }
        }

        public async Task<List<Appointment>> GetByBarberEmailAsync(string barberEmail)
        {
            var list = new List<Appointment>();
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_GetAppointmentsByBarber", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", barberEmail);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(Map(reader));
                            }
                        }
                    }
                }
                return list;
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error fetching barber history ({barberEmail}): {ex.Message}");
                throw;
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_DeleteAppointment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                AppLogger.Info($"Appointment {id} deleted from DB.");
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error deleting Appointment {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_GetAppointmentById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        await conn.OpenAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return Map(reader);
                            }
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error fetching Appointment {id}: {ex.Message}");
                throw;
            }
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
                
                Status = (AppointmentStatus)Convert.ToInt32(reader["Status"])
            };

            
            try { appt.BarberName = reader["BarberName"].ToString(); } catch { }
            try { appt.ClientName = reader["ClientName"].ToString(); } catch { }

            return appt;
        }
    }
}