using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class UserAccount
{
    public decimal Balance { get; set; }
    public List<decimal> DepositHistory { get; set; }
    public decimal FreeBetAmount { get; set; }
    public string BankAccountNumber { get; set; }
    public bool IsVerified { get; set; }

    public UserAccount()
    {
        DepositHistory = new List<decimal>();
    }

    public bool VerifyAccount(string bankAccountNumber)
    {
        if (bankAccountNumber.Length == 26 && bankAccountNumber.All(char.IsDigit))
        {
            BankAccountNumber = bankAccountNumber;
            IsVerified = true;
            return true;
        }
        else
        {
            IsVerified = false;
            return false;
        }
    }

    public void AddFreeBetIfThursday(decimal depositAmount)
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
        {
            FreeBetAmount = depositAmount * 0.5m;
            Balance += FreeBetAmount;
            Console.WriteLine($"CZWARTEK! 50% FREEBET od wpłaty! Freebet: {FreeBetAmount} zł.");
        }
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Kwota wpłaty musi być większa od zera.");
            return;
        }

        Balance += amount;
        DepositHistory.Add(amount);
        Console.WriteLine($"Wpłata {amount:0.00} zł została zrealizowana.");
    }

    public bool Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Kwota wypłaty musi być większa od zera.");
            return false;
        }

        if (FreeBetAmount > 0)
        {
            decimal requiredAmount = FreeBetAmount * 2;

            if (amount < requiredAmount)
            {
                Console.WriteLine($"Aby wypłacić freebet, musisz wypłacić co najmniej {requiredAmount:0.00} zł (2x freebet).");
                return false;
            }

            if (amount > Balance)
            {
                Console.WriteLine("Niewystarczające środki na wypłatę z freebetem.");
                return false;
            }

            Balance -= amount;
            FreeBetAmount = 0;
            Console.WriteLine($"Wypłata {amount:0.00} zł z freebetem zakończona.");
            return true;
        }
        else
        {
            if (amount > Balance)
            {
                Console.WriteLine("Niewystarczające środki na koncie.");
                return false;
            }

            Balance -= amount;
            Console.WriteLine($"Wypłata {amount:0.00} zł zakończona.");
            return true;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var user = new UserAccount();

        Console.WriteLine("=== WERYFIKACJA KONTA BANKOWEGO ===");

        while (!user.IsVerified)
        {
            Console.Write("Podaj numer konta bankowego (26 cyfr): ");
            string bankAccountNumber = Console.ReadLine();

            if (user.VerifyAccount(bankAccountNumber))
            {
                Console.WriteLine("WERYFIKACJA ZAKOŃCZONA POMYŚLNIE.");
            }
            else
            {
                Console.WriteLine("Nieprawidłowy numer konta. Spróbuj ponownie.");
            }
        }

        Console.WriteLine("\n=== WITAJ W SYSTEMIE PŁATNOŚCI ===");

        while (true)
        {
            Console.WriteLine($"\nSaldo: {user.Balance:0.00} zł (freebet: {user.FreeBetAmount:0.00} zł)");
            Console.WriteLine("1. Wpłata");
            Console.WriteLine("2. Wypłata");
            Console.WriteLine("0. Zakończ");
            Console.Write("Wybierz opcję: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Niepoprawny wybór.");
                continue;
            }

            if (choice == 0)
            {
                Console.WriteLine("Dziękujemy za skorzystanie. Do zobaczenia!");
                break;
            }

            switch (choice)
            {
                case 1:
                    HandleDeposit(user);
                    break;
                case 2:
                    HandleWithdraw(user);
                    break;
                default:
                    Console.WriteLine("Niepoprawny wybór.");
                    break;
            }
        }
    }

    static void HandleDeposit(UserAccount user)
    {
        Console.WriteLine("\nWybierz metodę płatności:");
        Console.WriteLine("1. KOD BLIK");
        Console.WriteLine("2. PRZELEW Z KONTA BANKOWEGO");

        string methodInput = Console.ReadLine();
        if (!int.TryParse(methodInput, out int paymentMethod) || (paymentMethod != 1 && paymentMethod != 2))
        {
            Console.WriteLine("Niepoprawna metoda płatności.");
            return;
        }

        if (paymentMethod == 1)
        {
            Console.Write("Wpisz 6-cyfrowy kod BLIK: ");
            string blikCode = Console.ReadLine();

            if (blikCode.Length == 6 && blikCode.All(char.IsDigit))
            {
                // Usunięty komunikat o akceptacji kodu BLIK
                Console.WriteLine("Czekam na weryfikację...");
                Thread.Sleep(7000); // Czekaj 7 sekund
                Console.WriteLine("BLIK zatwierdzony.");
            }
            else
            {
                Console.WriteLine("Nieprawidłowy kod BLIK.");
                return;
            }
        }
        else if (paymentMethod == 2)
        {
            Console.WriteLine("Z którego konta chcesz wykonać przelew?");
            Console.WriteLine($"1. Z WŁASNEGO konta: {user.BankAccountNumber}");
            Console.WriteLine("2. Z INNEGO konta");

            Console.Write("Twój wybór: ");
            string accountChoiceInput = Console.ReadLine();
            if (!int.TryParse(accountChoiceInput, out int accountChoice) || (accountChoice != 1 && accountChoice != 2))
            {
                Console.WriteLine("Niepoprawny wybór konta.");
                return;
            }

            if (accountChoice == 1)
            {
                Console.WriteLine($"Użyto Twojego konta: {user.BankAccountNumber}");
            }
            else
            {
                Console.Write("Wpisz numer konta bankowego (26 cyfr): ");
                string newAccount = Console.ReadLine();

                if (newAccount.Length != 26 || !newAccount.All(char.IsDigit))
                {
                    Console.WriteLine("Nieprawidłowy numer konta.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Użyto innego konta: {newAccount}");
                }
            }
        }

        Console.Write("Podaj kwotę wpłaty (zł, np. 45.50): ");
        string amountInput = Console.ReadLine();

        if (!decimal.TryParse(amountInput, out decimal depositAmount) || depositAmount <= 0)
        {
            Console.WriteLine("Niepoprawna kwota.");
            return;
        }

        user.Deposit(depositAmount);
        user.AddFreeBetIfThursday(depositAmount);
    }

    static void HandleWithdraw(UserAccount user)
    {
        Console.WriteLine($"\nSaldo: {user.Balance:0.00} zł (freebet: {user.FreeBetAmount:0.00} zł)");

        Console.WriteLine("Wybierz konto do wypłaty:");
        Console.WriteLine($"1. Na TWOJE konto: {user.BankAccountNumber}");
        Console.WriteLine("2. Na INNE konto");

        Console.Write("Twój wybór: ");
        string withdrawChoice = Console.ReadLine();

        string selectedAccount = "";

        if (withdrawChoice == "1")
        {
            selectedAccount = user.BankAccountNumber;
            Console.WriteLine($"Wypłata na Twoje konto: {selectedAccount}");
        }
        else if (withdrawChoice == "2")
        {
            Console.Write("Wpisz numer konta do wypłaty (26 cyfr): ");
            string otherAccount = Console.ReadLine();

            if (otherAccount.Length != 26 || !otherAccount.All(char.IsDigit))
            {
                Console.WriteLine("Niepoprawny numer konta.");
                return;
            }

            selectedAccount = otherAccount;
            Console.WriteLine($"Wypłata na inne konto: {selectedAccount}");
        }
        else
        {
            Console.WriteLine("Niepoprawny wybór.");
            return;
        }

        Console.WriteLine($"\nSaldo na koncie: {user.Balance:0.00} zł");
        Console.Write("Wpisz kwotę wypłaty (zł): ");
        string amountInput = Console.ReadLine();

        if (!decimal.TryParse(amountInput, out decimal withdrawAmount))
        {
            Console.WriteLine("Niepoprawna kwota.");
            return;
        }

        if (user.Withdraw(withdrawAmount))
        {
            Console.WriteLine($"Kwota {withdrawAmount:0.00} zł została wysłana na konto: {selectedAccount}");
        }
    }
}
