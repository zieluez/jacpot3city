using System;
using System.Threading;

class Program
{
    static string[] symbols = { "1", "2", "3", "4", "5" };
    static Random random = new Random();

    static decimal balance = 0;
    static decimal bet = 0;

    static void Main(string[] args)
    {
        Console.Title = "Gra Sloty";

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Saldo: {balance:C2}");
            Console.WriteLine("=== MENU ===");
            Console.WriteLine("1. Wpłać pieniądze");
            Console.WriteLine("2. Ustaw stawkę");
            Console.WriteLine("3. Zakręć slotami");
            Console.WriteLine("4. Autospin");
            Console.WriteLine("5. Zasady gry");
            Console.WriteLine("6. Wyjdź");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DepositMoney();
                    break;
                case "2":
                    SetBet();
                    break;
                case "3":
                    ManualSpinLoop();
                    break;
                case "4":
                    Autospin();
                    break;
                case "5":
                    ShowRules();
                    break;
                case "6":
                    Console.WriteLine("Dziękujemy za grę!");
                    return;
                default:
                    Console.WriteLine("Niepoprawny wybór. Naciśnij dowolny klawisz.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void DepositMoney()
    {
        Console.Clear();
        Console.Write("Ile chcesz wpłacić? (np. 50.00): ");
        if (decimal.TryParse(Console.ReadLine(), out decimal deposit) && deposit > 0)
        {
            balance += deposit;
            Console.WriteLine($"Wpłacono {deposit:C2}. Saldo: {balance:C2}");
        }
        else
        {
            Console.WriteLine("Niepoprawna kwota.");
        }
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić.");
        Console.ReadKey();
    }

    static void SetBet()
    {
        Console.Clear();
        Console.Write("Podaj stawkę: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal newBet) && newBet > 0 && newBet <= balance)
        {
            bet = newBet;
            Console.WriteLine($"Ustawiono stawkę: {bet:C2}");
        }
        else
        {
            Console.WriteLine("Niepoprawna stawka lub za mało środków.");
        }
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić.");
        Console.ReadKey();
    }

    static void ManualSpinLoop()
    {
        if (bet <= 0)
        {
            Console.WriteLine("Najpierw ustaw stawkę.");
            Console.ReadKey();
            return;
        }

        while (true)
        {
            if (balance < bet)
            {
                Console.WriteLine("Za mało środków. Wróć do menu i wpłać pieniądze.");
                Console.ReadKey();
                return;
            }

            SpinOnce();

            Console.WriteLine();
            Console.WriteLine("[Enter] - zakręć ponownie");
            Console.WriteLine("[M] - wróć do menu");

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.M)
                return;
        }
    }

    static void Autospin()
    {
        if (bet <= 0)
        {
            Console.WriteLine("Najpierw ustaw stawkę.");
            Console.ReadKey();
            return;
        }

        Console.Clear();
        Console.Write("Ile autospinów chcesz wykonać?: ");
        if (int.TryParse(Console.ReadLine(), out int spins) && spins > 0)
        {
            Console.Clear();
            Console.WriteLine($"Uruchamiam {spins} autospinów...");
            Console.WriteLine("------------------------------------------");

            for (int i = 1; i <= spins; i++)
            {
                if (balance < bet)
                {
                    Console.WriteLine($"Spin {i}: Za mało środków. Autospin przerwany.");
                    break;
                }

                balance -= bet;

                string reel1 = symbols[random.Next(symbols.Length)];
                string reel2 = symbols[random.Next(symbols.Length)];
                string reel3 = symbols[random.Next(symbols.Length)];

                bool isWin = reel1 == reel2 && reel2 == reel3;
                decimal prize = isWin ? bet * 10 : 0;

                if (isWin) balance += prize;

                Console.WriteLine($"Spin {i,2}: | {reel1} | {reel2} | {reel3} | {(isWin ? $"Wygrałeś {prize:C2}" : "Przegrana")}");
            }

            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Saldo po autospinie: {balance:C2}");
        }
        else
        {
            Console.WriteLine("Niepoprawna liczba.");
        }

        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić.");
        Console.ReadKey();
    }

    static void SpinOnce()
    {
        Console.Clear();
        Console.WriteLine("Kręcenie slotami...");
        Console.WriteLine();

        int spinDuration = 1500; // czas kręcenia (ms)
        int delay = 50; // opóźnienie między zmianami symboli

        string[] reelSymbols = new string[3];
        int[] finalIndices = new int[3];

        int cursorTop = Console.CursorTop;

        for (int t = 0; t < spinDuration / delay; t++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (t >= (i * 5))
                {
                    finalIndices[i] = random.Next(symbols.Length);
                    reelSymbols[i] = symbols[finalIndices[i]];
                }
            }

            Console.SetCursorPosition(0, cursorTop);
            Console.Write("| ");
            for (int i = 0; i < 3; i++)
            {
                string s = symbols[random.Next(symbols.Length)];
                Console.Write((t >= (i * 5) ? reelSymbols[i] : s) + " | ");
            }

            Thread.Sleep(delay);
        }

        Console.WriteLine(); // nowa linia po zatrzymaniu

        balance -= bet;

        string r1 = reelSymbols[0];
        string r2 = reelSymbols[1];
        string r3 = reelSymbols[2];

        if (r1 == r2 && r2 == r3)
        {
            decimal prize = bet * 10;
            balance += prize;
            Console.WriteLine($"\nWYGRAŁEŚ {prize:C2}!");
        }
        else
        {
            Console.WriteLine("\nPrzegrana.");
        }

        Console.WriteLine($"Saldo: {balance:C2}");
    }

    static void ShowRules()
    {
        Console.Clear();
        Console.WriteLine("ZASADY GRY:");
        Console.WriteLine();
        Console.WriteLine("Celem gry jest uzyskanie 3 takich samych symboli (1–5).");
        Console.WriteLine("Wygrana: 10x stawka za 3 identyczne symbole.");
        Console.WriteLine("Przegrana: symbole są różne.");
        Console.WriteLine();
        Console.WriteLine("Autospin wykonuje automatycznie wiele spinów.");
        Console.WriteLine("W trybie ręcznym możesz kontynuować bez wracania do menu.");
        Console.WriteLine();
        Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić.");
        Console.ReadKey();
    }
}
