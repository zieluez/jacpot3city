using System;
using System.Collections.Generic;
using System.Linq;

public class UserAccount
{
    public decimal Balance { get; set; }
    public List<decimal> DepositHistory { get; set; }
    public decimal FreeBetAmount { get; set; }
    public string BankAccountNumber { get; set; }
    public bool IsVerified { get; set; }

    // Metoda do weryfikacji konta (sprawdzenie numeru konta bankowego)
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

    // Metoda do dodania freebetu w czwartki
    public void AddFreeBetIfThursday(decimal depositAmount)
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
        {
            // Dodajemy freebet w wysokości 50% ostatniej wpłaty
            FreeBetAmount = depositAmount * 0.5m;
            Balance += FreeBetAmount;
            Console.WriteLine($"W CZWARTKI 50% freebetu od kwoty wplaconej! Freebet: {FreeBetAmount} zł.");
        }
    }

    // Metoda do wpłaty
    public void Deposit(decimal amount)
    {
        Balance += amount;
        DepositHistory.Add(amount);
        Console.WriteLine($"Wpłata na kwotę {amount} zł została zakończona.");
    }

    // Metoda do wypłaty
    public bool Withdraw(decimal amount)
    {
        // Sprawdzamy, czy użytkownik chce wypłacić freebet
        if (FreeBetAmount > 0)
        {
            decimal requiredBalance = FreeBetAmount * 2;
            if (amount == FreeBetAmount)
            {
                Console.WriteLine("Musisz podwoić kwotę, aby wypłacić freebet.");
                return false;
            }
            else if (amount > Balance - FreeBetAmount)
            {
                Console.WriteLine("Nie masz wystarczających środków na koncie, aby wypłacić z freebetem.");
                return false;
            }
        }

        if (amount <= Balance)
        {
            Balance -= amount;
            Console.WriteLine($"Wypłata na kwotę {amount} zł została zakończona.");
            return true;
        }
        else
        {
            Console.WriteLine("Niewystarczające środki na koncie.");
            return false;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var user = new UserAccount
        {
            Balance = 0,
            DepositHistory = new List<decimal>(),
            FreeBetAmount = 0,
            IsVerified = false
        };

        // Weryfikacja konta bankowego
        while (!user.IsVerified)
        {
            Console.WriteLine("Wpisz swój numer konta bankowego (26 cyfr): ");
            string bankAccountNumber = Console.ReadLine();

            if (user.VerifyAccount(bankAccountNumber))
            {
                Console.WriteLine("WERYFIKACJA ZAKONCZONA POPRAWNIE.");
            }
            else
            {
                Console.WriteLine("WERYFIKACJA NIEUDANA! Wpisz konto bankowe jeszcze raz.");
            }
        }

        // Główne menu - Wpłata / Wypłata
        while (true)
        {
            Console.WriteLine($"\nTwoje saldo wynosi: {user.Balance} zł (freebet: {user.FreeBetAmount} zł).");
            Console.WriteLine("Wybierz opcję:");
            Console.WriteLine("1. Wpłata");
            Console.WriteLine("2. Wypłata");
            Console.WriteLine("0. Zakończ");
            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 0)
            {
                break;
            }

            switch (choice)
            {
                case 1: // Wpłata
                    Console.WriteLine("Wybierz metodę płatności:");
                    Console.WriteLine("1. KOD BLIK");
                    Console.WriteLine("2. PRZELEW KONTO BANKOWE");
                    int paymentMethod = Convert.ToInt32(Console.ReadLine());

                    if (paymentMethod == 1)
                    {
                        Console.WriteLine("Wpisz 6-cyfrowy kod BLIK: ");
                        string blikCode = Console.ReadLine();

                        // Weryfikacja kodu BLIK (6 cyfr)
                        if (blikCode.Length == 6 && blikCode.All(char.IsDigit))
                        {
                            Console.WriteLine("Kod BLIK zaakceptowany.");
                        }
                        else
                        {
                            Console.WriteLine("Niepoprawny kod BLIK. Spróbuj ponownie.");
                            break;
                        }
                    }
                    else if (paymentMethod == 2)
                    {
                        Console.WriteLine("Wpisz numer konta bankowego, z którego wykonasz przelew (26 cyfr): ");
                        string accountNumber = Console.ReadLine();
                        if (accountNumber.Length != 26 || !accountNumber.All(char.IsDigit))
                        {
                            Console.WriteLine("Niepoprawny numer konta bankowego.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Numer konta zaakceptowany.");
                        }
                    }

                    Console.WriteLine("Wpisz kwotę wpłaty w zł: ");
                    decimal depositAmount = Convert.ToDecimal(Console.ReadLine());

                    user.Deposit(depositAmount);
                    user.AddFreeBetIfThursday(depositAmount);
                    break;

                case 2: // Wypłata
                    Console.WriteLine($"Twoje saldo wynosi: {user.Balance} zł (freebet: {user.FreeBetAmount} zł).");

                    // Sprawdzamy, czy użytkownik może wypłacić pieniądze
                    if (user.FreeBetAmount > 0)
                    {
                        decimal requiredBalance = user.FreeBetAmount * 2;
                        Console.WriteLine($"Jeśli chcesz wypłacić freebet, musisz podwoić swoją wpłatę.");
                        Console.WriteLine("1. Wypłać freebet");
                        Console.WriteLine("2. Zrezygnuj z freebetu i wypłać tylko rzeczywistą wpłatę");
                        int freebetChoice = Convert.ToInt32(Console.ReadLine());

                        if (freebetChoice == 1)
                        {
                            Console.WriteLine("Wpisz kwotę, którą chcesz wypłacić (musi być podwójna wartość wpłaty z freebetem): ");
                            decimal freebetWithdrawAmount = Convert.ToDecimal(Console.ReadLine());

                            if (user.Withdraw(freebetWithdrawAmount))
                            {
                                user.FreeBetAmount = 0; // Po wypłacie freebetu resetujemy jego kwotę
                            }
                        }
                        else if (freebetChoice == 2)
                        {
                            Console.WriteLine("Wpisz kwotę, którą chcesz wypłacić z rzeczywistego salda: ");
                            decimal regularWithdrawAmount = Convert.ToDecimal(Console.ReadLine());
                            user.Withdraw(regularWithdrawAmount);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wpisz kwotę wypłaty w zł: ");
                        decimal withdrawAmount = Convert.ToDecimal(Console.ReadLine());
                        user.Withdraw(withdrawAmount);
                    }
                    break;

                default:
                    Console.WriteLine("Niepoprawny wybór. Spróbuj ponownie.");
                    break;
            }
        }

        Console.WriteLine("Zakończenie programu.");
    }
}
