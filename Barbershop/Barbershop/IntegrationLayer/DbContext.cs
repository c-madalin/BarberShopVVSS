using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbershop.IntegrationLayer
{
    internal sealed class DbContext
    {
        private const string ConnectionString = "Server=localhost;" +
                                                "Database=BarbershopVVSS;" +
                                                "Trusted_Connection=True;" +
                                                "TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
