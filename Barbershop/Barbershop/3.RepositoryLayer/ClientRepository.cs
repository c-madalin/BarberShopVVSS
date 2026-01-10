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
using System.Threading.Tasks;

namespace Barbershop.RepositoryLayer
{
    internal sealed class ClientRepository: IUserRepository<Client>
    {
        public async Task AddAsync(Client client)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_InsertClient", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", client.LastName);
                        cmd.Parameters.AddWithValue("@Email", client.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
                        cmd.Parameters.AddWithValue("@PasswordHash", client.PasswordHash);
                        cmd.Parameters.AddWithValue("@IsActive", client.IsActive);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error adding Client {client.Email}: {ex.Message}");
                throw;
            }
        }
        public async Task<Client?> GetByEmailAsync(string email)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_GetClientByEmail", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", email);

                        await conn.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Client
                                {
                                    Id = (int)reader["Id"],
                                    FirstName = (string)reader["FirstName"] ?? throw new InvalidInsertFieldException("FirstName cannot be null."),
                                    LastName = (string)reader["LastName"] ?? throw new InvalidInsertFieldException("LastName cannot be null."),
                                    Email = (string)reader["Email"] ?? throw new InvalidInsertFieldException("Email cannot be null."),
                                    PhoneNumber = (string)reader["PhoneNumber"] ?? throw new InvalidInsertFieldException("PhoneNumber cannot be null."),
                                    PasswordHash = (string)reader["PasswordHash"] ?? throw new InvalidInsertFieldException("PasswordHash cannot be null."),
                                    IsActive = (bool)reader["IsActive"]
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error fetching Client {email}: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateStatusAsync(string email, bool isActive)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_UpdateClientStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@IsActive", isActive);

                        await conn.OpenAsync();
                        await cmd.ExecuteReaderAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error updating status for Client {email}: {ex.Message}");
                throw;
            }
        }
        public async Task DeleteAsync(string email)
        {
            try
            {
                using (var conn = DbContext.CreateConnection())
                {
                    using (var cmd = new SqlCommand("sp_DeleteClient", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Email", email);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException ex)
            {
                AppLogger.Error($"Database error deleting Client {email}: {ex.Message}");
                throw;
            }
        }
    }
}
