using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreBankSystem
{
    internal class AccountOperation
    {
        public void AddAccount(int userId)
        {
            Console.WriteLine("Enter Account Holder's Name: ");
            string accountHolderName = Console.ReadLine();

            Console.WriteLine("Enter Current Balance: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal currentBalance))
            {
                using (var dbContext = new BankSysDBContext())
                {
                    try
                    {
                        // Create a new Account object
                        var newAccount = new Account
                        {
                            AccountHolderName = accountHolderName,
                            Balance = currentBalance,
                            UserId = userId // Associate the account with the logged-in user
                        };

                        // Add the new account to the DbContext
                        dbContext.Accounts.Add(newAccount);

                        // Save changes to the database
                        dbContext.SaveChanges();

                        Console.WriteLine("Account added successfully!");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: " + e.Message);
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input for Current Balance.");
                Console.WriteLine("---------------------------");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        public void ViewAccountsForUser(int userId)
        {
            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    // Query the accounts for the user using Entity Framework Core
                    var userAccounts = dbContext.Accounts
                        .Where(account => account.UserId == userId)
                        .ToList();

                    if (userAccounts.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Accounts for the current user:");
                        Console.ResetColor();

                        foreach (var account in userAccounts)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Account Number: {account.AccountNumber}");
                            Console.WriteLine($"Account Holder: {account.AccountHolderName}");
                            Console.WriteLine($"Current Balance: {account.Balance} OMR");
                            Console.WriteLine("---------------------------");
                            Console.ResetColor();
                        }

                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("No accounts found for this user.");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        public void Withdraw(int userId)
        {
            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    // Display the user's accounts and ask for the account number
                    ViewAccountsForUser(userId);
                    Console.WriteLine("Enter the Account Number from which you want to withdraw: ");
                    if (int.TryParse(Console.ReadLine(), out int accountNumber))
                    {
                        Console.WriteLine("Enter the amount to withdraw: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawalAmount))
                        {
                            // Check if the specified account belongs to the current user
                            var account = dbContext.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber && a.UserId == userId);

                            if (account != null)
                            {
                                decimal currentBalance = account.Balance;

                                if (currentBalance >= withdrawalAmount)
                                {
                                    // Update the balance with the new amount after withdrawal
                                    account.Balance -= withdrawalAmount;
                                    dbContext.SaveChanges();

                                    Console.WriteLine("Withdrawal successful!");
                                    RecordTransaction("Withdrawal", withdrawalAmount, accountNumber, null);
                                }
                                else
                                {
                                    Console.WriteLine("Insufficient funds in the selected account.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("The specified account does not belong to you.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for withdrawal amount.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input for account number.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        public void Deposit(int userId)
        {
            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    // Display the user's accounts and ask for the account number
                    ViewAccountsForUser(userId);
                    Console.WriteLine("Enter the Account Number to which you want to deposit: ");

                    if (int.TryParse(Console.ReadLine(), out int accountNumber))
                    {
                        Console.WriteLine("Enter the amount to deposit: ");

                        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                        {
                            // Check if the specified account belongs to the current user
                            var account = dbContext.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber && a.UserId == userId);

                            if (account != null)
                            {
                                decimal currentBalance = account.Balance;

                                // Update the balance with the new amount after deposit
                                account.Balance += depositAmount;
                                dbContext.SaveChanges();

                                Console.WriteLine("Deposit successful!");
                                RecordTransaction("Deposit", depositAmount, null, accountNumber);
                            }
                            else
                            {
                                Console.WriteLine("The specified account does not belong to you.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for deposit amount.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input for account number.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        public void Transfer(int userId)
        {
            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    // Display the user's accounts and ask for the source account number
                    Console.WriteLine("Transfer Money");
                    Console.WriteLine("Your Accounts:");
                    ViewAccountsForUser(userId);

                    Console.WriteLine("Enter the Account Number to transfer money from: ");

                    if (int.TryParse(Console.ReadLine(), out int sourceAccountNumber))
                    {
                        Console.WriteLine("Enter the target Account Number to transfer money to: ");

                        if (int.TryParse(Console.ReadLine(), out int targetAccountNumber))
                        {
                            Console.WriteLine("Enter the amount to transfer: ");

                            if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount))
                            {
                                // Check if the source account belongs to the current user
                                var sourceAccount = dbContext.Accounts.FirstOrDefault(a => a.AccountNumber == sourceAccountNumber && a.UserId == userId);

                                if (sourceAccount != null)
                                {
                                    if (sourceAccount.Balance >= transferAmount)
                                    {
                                        // Update the source account balance with the new amount after transfer
                                        sourceAccount.Balance -= transferAmount;

                                        // Check if the target account belongs to the current user or another user
                                        var targetAccount = dbContext.Accounts.FirstOrDefault(a => a.AccountNumber == targetAccountNumber);

                                        if (targetAccount != null)
                                        {
                                            if (targetAccount.UserId == userId)
                                            {
                                                // Transfer to own account
                                                targetAccount.Balance += transferAmount;
                                                dbContext.SaveChanges();

                                                Console.WriteLine("Transfer successful!");
                                                RecordTransaction("Transfer", transferAmount, sourceAccountNumber, targetAccountNumber);
                                            }
                                            else
                                            {
                                                Console.WriteLine("You can only transfer to your own accounts.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Target account not found.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Insufficient funds in the source account.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("The specified source account does not belong to you.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input for transfer amount.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input for target account number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input for source account number.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        private void RecordTransaction(string transactionType, decimal amount, int? sourceAccountNumber, int? targetAccountNumber)
        {
            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    var transaction = new Transaction
                    {
                        Timestamp = DateTime.Now,
                        Type = transactionType,
                        Amount = amount,
                        SrcAccNO = sourceAccountNumber,
                        TargetAccNO = targetAccountNumber
                    };

                    dbContext.Transactions.Add(transaction);
                    dbContext.SaveChanges();

                    Console.WriteLine("Transaction recorded successfully.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred while recording the transaction: " + e.Message);
                }
            }
        }

        public void ViewTransactionHistory(int userId, string period)
        {
            DateTime minSqlDate = new DateTime(1753, 1, 1);
            DateTime startDate;

            switch (period.ToLower())
            {
                case "last transaction":
                    startDate = minSqlDate; // Set to minimum date
                    break;
                case "last day":
                    startDate = DateTime.Now.AddDays(-1);
                    break;
                case "last 5 days":
                    startDate = DateTime.Now.AddDays(-5);
                    break;
                case "last 1 month":
                    startDate = DateTime.Now.AddMonths(-1);
                    break;
                case "last 2 months":
                    startDate = DateTime.Now.AddMonths(-2);
                    break;
                default:
                    Console.WriteLine("Invalid period. Showing all transactions.");
                    startDate = minSqlDate; // Set to minimum date
                    break;
            }

            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    // Define the LINQ query to fetch transaction history for the user's accounts within the specified period
                    var transactions = dbContext.Transactions
                        .Where(t => (t.SrcAccNO.HasValue && dbContext.Accounts.Any(a => a.UserId == userId && a.AccountNumber == t.SrcAccNO.Value))
                                    || (t.TargetAccNO.HasValue && dbContext.Accounts.Any(a => a.UserId == userId && a.AccountNumber == t.TargetAccNO.Value))
                                    && t.Timestamp >= startDate)
                        .OrderByDescending(t => t.Timestamp)
                        .ToList();

                    if (transactions.Any())
                    {
                        Console.WriteLine($"Transaction History (Last {period}):");
                        foreach (var transaction in transactions)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Transaction ID: {transaction.TransId}");
                            Console.WriteLine($"Timestamp: {transaction.Timestamp}");
                            Console.WriteLine($"Type: {transaction.Type}");
                            Console.WriteLine($"Amount: {transaction.Amount} OMR");
                            Console.ResetColor();

                            if (transaction.SrcAccNO.HasValue)
                            {
                                Console.WriteLine($"Source Account: {transaction.SrcAccNO}");
                            }

                            if (transaction.TargetAccNO.HasValue)
                            {
                                Console.WriteLine($"Target Account: {transaction.TargetAccNO}");
                            }

                            Console.WriteLine("---------------------------");
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("No transaction history found.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public void DeleteUser(int userId)
        {
            Console.WriteLine("Enter your email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password: ");
            string password = Console.ReadLine();

            using (var dbContext = new BankSysDBContext())
            {
                try
                {
                    var userToDelete = dbContext.Users
                        .FirstOrDefault(u => u.UserId == userId && u.Email == email && u.Password == password);

                    if (userToDelete != null)
                    {
                        using (var transaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                // Delete user's transactions
                                var userAccountNumbers = dbContext.Accounts
                                    .Where(a => a.UserId == userId)
                                    .Select(a => a.AccountNumber)
                                    .ToList();

                                dbContext.Transactions
                                    .RemoveRange(dbContext.Transactions
                                        .Where(t => userAccountNumbers.Contains(t.SrcAccNO ?? 0) || userAccountNumbers.Contains(t.TargetAccNO ?? 0)));

                                // Delete user's accounts
                                dbContext.Accounts.RemoveRange(dbContext.Accounts.Where(a => a.UserId == userId));

                                // Delete the user
                                dbContext.Users.Remove(userToDelete);

                                // Save changes to commit the transaction
                                dbContext.SaveChanges();

                                transaction.Commit();

                                Console.WriteLine("User deleted successfully.");
                                Console.WriteLine("---------------------------");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("An error occurred while deleting the user, accounts, and transactions: " + e.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid email or password. Deletion failed.");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }


            }
        }
        public void DeleteUserAccount(int userId)
{
    // Display the user's accounts
    ViewAccountsForUser(userId);

    Console.WriteLine("Enter the Account Number you want to delete: ");

    if (int.TryParse(Console.ReadLine(), out int accountNumberToDelete))
    {
        Console.WriteLine("Enter your email: ");
        string email = Console.ReadLine();

        Console.WriteLine("Enter your password: ");
        string password = Console.ReadLine();

        using (var dbContext = new BankSysDBContext())
        {
            try
            {
                var accountToDelete = dbContext.Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumberToDelete && a.UserId == userId);

                if (accountToDelete != null)
                {
                    var userToDelete = dbContext.Users
                        .FirstOrDefault(u => u.UserId == userId && u.Email == email && u.Password == password);

                    if (userToDelete != null)
                    {
                        using (var transaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                // Remove the account from the user's accounts
                                dbContext.Accounts.Remove(new Account { UserId = userId, AccountNumber = accountNumberToDelete });

                                // Save changes to commit the transaction
                                dbContext.SaveChanges();

                                transaction.Commit();

                                Console.WriteLine("Account deleted successfully.");
                                Console.WriteLine("---------------------------");
                                Console.WriteLine("Press any key to continue...");
                                Console.ReadKey();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("An error occurred while deleting the account: " + e.Message);
                                transaction.Rollback();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid email or password. Deletion failed.");
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("The specified account does not belong to you.");
                    Console.WriteLine("---------------------------");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }
    }
    else
    {
        Console.WriteLine("Invalid input for account number.");
        Console.WriteLine("---------------------------");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}

    }
}
