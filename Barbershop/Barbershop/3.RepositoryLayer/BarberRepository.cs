using Barbershop.EntityLayer;
using Barbershop.IntegrationLayer;
using Barbershop.Utils.Exceptions;
using Barbershop.Utils.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    internal sealed class BarberRepository: IUserRepository<Barber>
    {
        public async Task AddAsync(Barber barber)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertBarber", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FirstName", barber.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", barber.LastName);
                        cmd.Parameters.AddWithValue("@Email", barber.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", barber.PhoneNumber);
                        cmd.Parameters.AddWithValue("@PasswordHash", barber.PasswordHash);
                        cmd.Parameters.AddWithValue("@IsActive", barber.IsActive);
                        cmd.Parameters.AddWithValue("@Specialisation", barber.Specialisation);
                        cmd.Parameters.AddWithValue("@Salary", barber.Salary);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error adding Barber {barber.Email}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Unexpected error adding Barber {barber.Email}: {ex.Message}");
                throw;
            }
        }
        public async Task<Barber?> GetByEmailAsync(string email)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_GetBarberByEmail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", email);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Barber
                                {
                                    Id = (int)reader["Id"],
                                    FirstName = (string)reader["FirstName"] ?? throw new InvalidInsertFieldException("FirstName cannot be null."),
                                    LastName = (string)reader["LastName"] ?? throw new InvalidInsertFieldException("LastName cannot be null."),
                                    Email = (string)reader["Email"] ?? throw new InvalidInsertFieldException("Email cannot be null."),
                                    PhoneNumber = (string)reader["PhoneNumber"] ?? throw new InvalidInsertFieldException("PhoneNumber cannot be null."),
                                    PasswordHash = (string)reader["PasswordHash"] ?? throw new InvalidInsertFieldException("PasswordHash cannot be null."),
                                    IsActive = (bool)reader["IsActive"],
                                    Specialisation = (string)reader["Specialisation"] ?? throw new InvalidInsertFieldException("FirstName cannot be null."),
                                    Salary = (decimal)reader["Salary"]
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error fetching Barber {email}: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateStatusAsync(string email, bool isActive)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_UpdateBarberStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                AppLogger.Info($"Barber status updated in DB: {email}");
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error updating status for {email}: {ex.Message}");
                throw;
            }
        }
        public async Task DeleteAsync(string email)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_DeleteBarber", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", email);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                AppLogger.Info($"Barber deleted from DB: {email}");
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error deleting Barber {email}: {ex.Message}");
                throw;
            }
        }
    }
}
