using Barbershop._1.ServiceLayer.DTOs;
using Barbershop.DomainLayer;
using Barbershop.EntityLayer;
using Barbershop.NetworkingLayer;
using Barbershop.RepositoryLayer;
using Barbershop.ServiceLayer;
using Barbershop.Utils.Logging;
using Barbershop.Utils.Logging.Enum;
using Barbershop.Utils.Logging.Interface;
using System;
using System.Threading.Tasks;

namespace Barbershop
{
    class Program
    {
        public class ConsoleLogger : IAppLogger
        {
            public void Log(string message, LogLevel level)
            {
                var color = level switch
                {
                    LogLevel.Info => ConsoleColor.Green,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    _ => ConsoleColor.White
                };
                Console.ForegroundColor = color;
                Console.WriteLine($"[{level}] {message}");
                Console.ResetColor();
            }
        }

        static async Task Main(string[] args)
        {
            AppLogger.Init(new ConsoleLogger());
            Console.WriteLine("=== Starting Barbershop System Test ===\n");

            IEmailVerifier emailVerifier = new EmailVerifier();

            var barberRepo = new BarberRepository();
            var barberDomain = new BarberDomain(barberRepo);
            var barberService = new BarberService(barberDomain, emailVerifier);

            var clientRepo = new ClientRepository();
            var clientDomain = new ClientDomain(clientRepo);
            var clientService = new ClientService(clientDomain, emailVerifier);

            try
            {
                Console.WriteLine("\n--- Testing Barber Workflow ---");

                string bEmail = $"tony.stark{new Random().Next(100, 999)}@avengers.com";
                string bPass = "IronMan123!";

                Console.WriteLine($"\n1. Registering Barber: {bEmail}...");
                await barberService.NewRegisterAsync(new BarberRegisterDto(
                    FirstName: "Tony",
                    LastName: "Stark",
                    Email: bEmail,
                    Phone: "123-456-7890",
                    Password: bPass,
                    Specialisation: "Beard Trimming",
                    Salary: 5000)
                );

                Console.WriteLine("\n2. Logging in...");
                var loggedBarber = await barberService.LoginAsync(bEmail, bPass);
                Console.WriteLine($"   Login Success! Hello, {loggedBarber.FirstName} {loggedBarber.LastName}.");

                Console.WriteLine("\n3. Updating Status (Deactivating)...");
                await barberService.UpdateStatusAsync(bEmail);
                Console.WriteLine("   Status update request sent.");

                Console.WriteLine("\n4. Deleting User...");
                await barberService.DeleteAsync(bEmail);
                Console.WriteLine("   Delete request sent.");

                Console.WriteLine("\n\n--- Testing Client Workflow ---");

                string cEmail = $"steve.rogers{new Random().Next(100, 999)}@avengers.com";
                string cPass = "CapAmerica1!";

                Console.WriteLine($"\n1. Registering Client: {cEmail}...");
                await clientService.NewRegisterAsync(new ClientRegisterDto(
                    FirstName: "Steve",
                    LastName: "Rogers",
                    Email: cEmail,
                    Phone: "987-654-3210",
                    Password: cPass)
                );

                Console.WriteLine("\n2. Logging in...");
                var loggedClient = await clientService.LoginAsync(cEmail, cPass);
                Console.WriteLine($"   Login Success! Hello, {loggedClient.FirstName}.");

                Console.WriteLine("\n3. Cleaning up Client...");
                await clientService.DeleteAsync(cEmail);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[CRITICAL FAILURE]: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }

            Console.WriteLine("\n=== Test Complete. Press any key to exit. ===");
            Console.ReadKey();
        }
    }
}
