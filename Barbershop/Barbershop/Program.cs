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
using System.Collections.Generic;

namespace Barbershop
{
    class Program
    {
        public class ConsoleLogger : IAppLogger
        {
            public void Log(string message, LogLevel level)
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = level switch
                {
                    LogLevel.Info => ConsoleColor.Green,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    _ => ConsoleColor.White
                };
                Console.WriteLine($"[{level}] {message}");
                Console.ForegroundColor = originalColor;
            }
        }

        static IAppointmentService _appointmentService;
        static IReviewService _reviewService;
        static BarberService _barberService;
        static ClientService _clientService;

        static async Task Main(string[] args)
        {
            AppLogger.Init(new ConsoleLogger());
            Console.WriteLine("Initializing BarberShop System...");

            var emailVerifier = new EmailVerifier();

            var barberRepo = new BarberRepository();
            var clientRepo = new ClientRepository();
            var apptRepo = new AppointmentRepository();
            var reviewRepo = new ReviewRepository();

            var barberDomain = new BarberDomain(barberRepo);
            var clientDomain = new ClientDomain(clientRepo);
            var apptDomain = new AppointmentDomain(apptRepo, clientRepo, barberRepo);
            var reviewDomain = new ReviewDomain(reviewRepo, apptRepo);

            _barberService = new BarberService(barberDomain, emailVerifier);
            _clientService = new ClientService(clientDomain, emailVerifier);
            _appointmentService = new AppointmentService(apptDomain);
            _reviewService = new ReviewService(reviewDomain);

            Console.WriteLine("System Ready.\n");

            while (true)
            {
                Console.WriteLine("\n=== BARBERSHOP MAIN MENU ===");
                Console.WriteLine("1. Login as CLIENT");
                Console.WriteLine("2. Login as BARBER");
                Console.WriteLine("3. Register New CLIENT (Quick Setup)");
                Console.WriteLine("4. Register New BARBER (Quick Setup)");
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");

                var key = Console.ReadLine();
                Console.Clear();

                try
                {
                    switch (key)
                    {
                        case "1":
                            await LoginClientFlow();
                            break;
                        case "2":
                            await LoginBarberFlow();
                            break;
                        case "3":
                            await RegisterClientFlow();
                            break;
                        case "4":
                            await RegisterBarberFlow();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"System Error: {ex.Message}");
                }
            }
        }

        static async Task LoginClientFlow()
        {
            Console.WriteLine("--- CLIENT LOGIN ---");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string pass = Console.ReadLine();

            try
            {
                var client = await _clientService.LoginAsync(email, pass);
                Console.WriteLine($"\nWelcome back, {client.FirstName}!");
                await ClientMenu(client);
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
            }
        }

        static async Task ClientMenu(Client client)
        {
            while (true)
            {
                Console.WriteLine($"\n=== CLIENT MENU ({client.Email}) ===");
                Console.WriteLine("1. Book Appointment");
                Console.WriteLine("2. View My History");
                Console.WriteLine("3. Leave a Review");
                Console.WriteLine("0. Logout");
                Console.Write("Option: ");

                var opt = Console.ReadLine();
                Console.Clear();

                if (opt == "0") break;

                try
                {
                    switch (opt)
                    {
                        case "1":
                            Console.Write("Barber Email: ");
                            var bEmail = Console.ReadLine();
                            Console.Write("Date (yyyy-mm-dd hh:mm): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                            {
                                Console.Write("Service Type (Haircut/Beard/Fade): ");
                                var service = Console.ReadLine();
                                await _appointmentService.CreateAppointmentAsync(client.Email, bEmail, date, service);
                            }
                            else
                            {
                                Console.WriteLine("Invalid date format.");
                            }
                            break;

                        case "2":
                            var history = await _appointmentService.GetHistoryClientAsync(client.Email);
                            Console.WriteLine("\n--- Your Appointments ---");
                            foreach (var appt in history)
                            {
                                Console.WriteLine($"ID: {appt.AppointmentID} | Date: {appt.AppointmentDate} | Barber: {appt.BarberName} | Service: {appt.ServiceType}");
                            }
                            break;

                        case "3":
                            Console.WriteLine("You must know the Appointment ID to leave a review (Check History first).");
                            Console.Write("Appointment ID: ");
                            if (int.TryParse(Console.ReadLine(), out int appId))
                            {
                                Console.Write("Rating (1-5): ");
                                int rating = int.Parse(Console.ReadLine());
                                Console.Write("Comment: ");
                                string comment = Console.ReadLine();
                                Console.Write("Confirm Barber Email: ");
                                string bMail = Console.ReadLine();

                                await _reviewService.AddReviewAsync(appId, client.Email, bMail, rating, comment);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"Operation failed: {ex.Message}");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static async Task RegisterClientFlow()
        {
            Console.Write("First Name: "); var fn = Console.ReadLine();
            Console.Write("Last Name: "); var ln = Console.ReadLine();
            Console.Write("Email: "); var em = Console.ReadLine();
            Console.Write("Password: "); var pw = Console.ReadLine();
            Console.Write("Phone: "); var ph = Console.ReadLine();

            var dto = new ClientRegisterDto(fn, ln, em, ph, pw);
            await _clientService.NewRegisterAsync(dto);
        }

        static async Task LoginBarberFlow()
        {
            Console.WriteLine("--- BARBER LOGIN ---");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string pass = Console.ReadLine();

            try
            {
                var barber = await _barberService.LoginAsync(email, pass);
                Console.WriteLine($"\nWelcome back, Master Barber {barber.LastName}!");
                await BarberMenu(barber);
            }
            catch (Exception ex)
            {
                AppLogger.Error(ex.Message);
            }
        }

        static async Task BarberMenu(Barber barber)
        {
            while (true)
            {
                Console.WriteLine($"\n=== BARBER MENU ({barber.Email}) ===");
                Console.WriteLine("1. View My Appointments");
                Console.WriteLine("2. View My Reviews");
                Console.WriteLine("0. Logout");
                Console.Write("Option: ");

                var opt = Console.ReadLine();
                Console.Clear();

                if (opt == "0") break;

                try
                {
                    switch (opt)
                    {
                        case "1":
                            var history = await _appointmentService.GetHistoryBarberAsync(barber.Email);
                            Console.WriteLine("\n--- Upcoming Appointments ---");
                            foreach (var appt in history)
                            {
                                Console.WriteLine($"Date: {appt.AppointmentDate} | Client: {appt.ClientName} ({appt.CustomerEmail}) | Service: {appt.ServiceType}");
                            }
                            break;

                        case "2":
                            var reviews = await _reviewService.GetReviewsForBarberAsync(barber.Email);
                            Console.WriteLine("\n--- Client Feedback ---");
                            foreach (var r in reviews)
                            {
                                Console.WriteLine($"[*] Rating: {r.Rating}/5");
                                Console.WriteLine($"    From: {r.ClientEmail}");
                                Console.WriteLine($"    Comment: {r.Comment}");
                                Console.WriteLine("    ------------------");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error($"Operation failed: {ex.Message}");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static async Task RegisterBarberFlow()
        {
            Console.Write("First Name: "); var fn = Console.ReadLine();
            Console.Write("Last Name: "); var ln = Console.ReadLine();
            Console.Write("Email: "); var em = Console.ReadLine();
            Console.Write("Password: "); var pw = Console.ReadLine();
            Console.Write("Salary: "); decimal sal = decimal.Parse(Console.ReadLine());
            Console.Write("Specialisation: "); var spec = Console.ReadLine();

            var dto = new BarberRegisterDto(fn, ln, em, "0000000000", pw, spec, sal);
            await _barberService.NewRegisterAsync(dto);
        }
    }
}
