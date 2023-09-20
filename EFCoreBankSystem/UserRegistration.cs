using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFCoreBankSystem
{
    internal class UserRegistration
    {
        private CurrentUser currentUser;
        public void RegisterUser()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Enter your email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            using (var dbContext = new BankSysDBContext())
            {
                if (IsPasswordValid(password))
                {
                    var newUser = new User
                    {
                        Name = name,
                        Email = email,
                        Password = password
                    };

                    dbContext.Users.Add(newUser);
                    dbContext.SaveChanges();

                    Console.WriteLine("Registration successful!");
                }
                else
                {
                    Console.WriteLine("Password is invalid.");
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private bool IsPasswordValid(string password)
        {
            // Define the regular expression pattern
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";

            // Create a regex object
            Regex regex = new Regex(pattern);

            // Use regex.IsMatch to check if the password matches the pattern
            return regex.IsMatch(password);
        }
        public async Task<bool> UserLogin()
        {
            Console.WriteLine("Enter your email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            using (var dbContext = new BankSysDBContext())
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                if (user != null)
                {
                    // User exists, set currentUser to the user's information
                    currentUser = new CurrentUser
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Email = user.Email
                    };

                    Console.WriteLine("Login successful!");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return true; // User exists, login successful
                }
                else
                {
                    Console.WriteLine("Login failed. Check your email and password.");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return false; // Email or password is incorrect
                }
            }
        }

        public CurrentUser GetCurrentUser()
        {
            return currentUser;
        }

    }
}
