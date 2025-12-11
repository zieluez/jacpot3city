using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static readonly int[] czerwone = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
    static readonly int[] czarne = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };

    static readonly int[,] plansza = new int[12, 3]
    {
        {1,2,3},
        {4,5,6},
        {7,8,9},
        {10,11,12},
        {13,14,15},
        {16,17,18},
        {19,20,21},
        {22,23,24},
        {25,26,27},
        {28,29,30},
        {31,32,33},
        {34,35,36}
    };

    static void Main()
    {
        Random rng = new Random();
        double saldo = PobierzPoczatkoweSaldo();

        while (true)
        {
            Console.WriteLine($"\n--- NOWA TURA ---");
            Console.WriteLine($"Twoje saldo: {saldo:F2} PLN");

            var zaklady = new List<Zaklad>();

            while (true)
            {
                WyswietlMenu();
                string wybor = Console.ReadLine();

                if (wybor == "0")
                {
                    if (zaklady.Count == 0)
                    {
                        Console.WriteLine("Musisz postawić przynajmniej jeden zakład.");
                        continue;
                    }
                    break;
                }

                Zaklad z = StworzZaklad(wybor, saldo);
                if (z != null)
                {
                    saldo -= z.kwota;
                    Console.WriteLine($"Zakład przyjęty. Nowe saldo: {saldo:F2} PLN");
                    zaklady.Add(z);
                }
            }

            int wynik = rng.Next(0, 37);
            string kolor = KolorLiczby(wynik);

            Console.WriteLine($"\n*** Wylosowana liczba: {wynik} ({kolor}) ***");

            foreach (var z in zaklady)
            {
                if (z.CzyWygrana(wynik))
                {
                    double wygrana = z.kwota * z.wyplata;
                    Console.WriteLine($"WYGRANA! ({z.opis}) -> +{wygrana:F2} PLN");
                    saldo += wygrana;
                }
                else
                {
                    Console.WriteLine($"Przegrana ({z.opis})");
                }
            }

            Console.WriteLine($"\nSaldo po turze: {saldo:F2} PLN");

            if (saldo <= 0)
            {
                Console.WriteLine("Saldo wynosi 0. Koniec gry!");
                break;
            }

            Console.WriteLine("Grasz dalej? (t/n)");
            if (Console.ReadLine().ToLower() != "t")
                break;
        }
    }

    class Zaklad
    {
        public string opis;
        public double kwota;
        public Func<int, bool> CzyWygrana;
        public int wyplata;

        public Zaklad(string opis, double kwota, Func<int, bool> f, int wyplata)
        {
            this.opis = opis;
            this.kwota = kwota;
            this.CzyWygrana = f;
            this.wyplata = wyplata;
        }
    }

    static void WyswietlMenu()
    {
        Console.WriteLine("\nWybierz typ zakładu:");
        Console.WriteLine("----- ZAKŁADY WEWNĘTRZNE -----");
        Console.WriteLine("1 - Pojedyncza liczba");
        Console.WriteLine("2 - Split (2 liczby obok siebie)");
        Console.WriteLine("3 - Corner (4 liczby w kwadracie)");
        Console.WriteLine("\n----- ZAKŁADY ZEWNĘTRZNE -----");
        Console.WriteLine("4 - Kolor (czerwony / czarny / zielony)");
        Console.WriteLine("5 - Parzyste/nieparzyste");
        Console.WriteLine("6 - Małe/duże (1–18 / 19–36)");
        Console.WriteLine("7 - Tuzy (1–12 / 13–24 / 25–36)");
        Console.WriteLine("8 - Kolumny (1. / 2. / 3.)");
        Console.WriteLine("------------------------");
        Console.WriteLine("0 - Zakończ obstawianie");
    }

    static Zaklad StworzZaklad(string wybor, double saldo)
    {
        double kwota = PobierzKwoteZakladu(saldo);
        if (kwota == -1) return null;

        switch (wybor)
        {
            case "1": // Pojedyncza liczba
                Console.WriteLine("Podaj liczbę (0-36):");
                int n = PobierzLiczbe();
                return new Zaklad($"Pojedyncza liczba {n}", kwota, (wynik => wynik == n), 35);

            case "2": // Split
                Console.WriteLine("Podaj dwie liczby obok siebie (np. 1 2):");
                var s = PobierzListeLiczb(2);
                if (!CzySplitPoprawny(s))
                {
                    Console.WriteLine("Niepoprawny split. Liczby muszą być obok siebie.");
                    return null;
                }
                return new Zaklad($"Split {s[0]}/{s[1]}", kwota, (wynik => s.Contains(wynik)), 17);

            case "3": // Corner
                Console.WriteLine("Podaj cztery liczby w kwadracie (np. 1 2 4 5):");
                var c = PobierzListeLiczb(4);
                if (!CzyCornerPoprawny(c))
                {
                    Console.WriteLine("Niepoprawny corner. Liczby muszą tworzyć kwadrat.");
                    return null;
                }
                return new Zaklad($"Corner {string.Join(",", c)}", kwota, (wynik => c.Contains(wynik)), 8);

            case "4": // Kolor
                Console.WriteLine("Wybierz kolor (czerwony / czarny / zielony):");
                string kolor = Console.ReadLine().ToLower();
                if (kolor != "czerwony" && kolor != "czarny" && kolor != "zielony") return null;
                return new Zaklad($"Kolor {kolor}", kwota, (wynik => KolorLiczby(wynik) == kolor), 2);

            case "5": // Parzyste/nieparzyste
                Console.WriteLine("Wybierz parzyste / nieparzyste:");
                string p = Console.ReadLine().ToLower();
                return new Zaklad($"Parzyste/nieparzyste {p}", kwota,
                    (wynik => wynik != 0 && ((wynik % 2 == 0 && p == "parzyste") || (wynik % 2 == 1 && p == "nieparzyste"))),
                    2);

            case "6": // Małe/duże
                Console.WriteLine("Wybierz zakres (1-18 / 19-36):");
                string zakres = Console.ReadLine();
                return new Zaklad($"Małe/duże {zakres}", kwota,
                    (wynik => (zakres == "1-18" && wynik >= 1 && wynik <= 18) || (zakres == "19-36" && wynik >= 19 && wynik <= 36)),
                    2);

            case "7": // Tuzy
                Console.WriteLine("Wybierz tuzy (1-12 / 13-24 / 25-36):");
                string dz = Console.ReadLine();
                return new Zaklad($"Tuzy {dz}", kwota, (wynik => CheckDozen(wynik, dz)), 3);

            case "8": // Kolumny
                Console.WriteLine("Wybierz kolumnę (1 / 2 / 3):");
                if (!int.TryParse(Console.ReadLine(), out int col)) return null;
                return new Zaklad($"Kolumna {col}", kwota, (wynik => wynik != 0 && ((wynik - col) % 3 == 0)), 3);
        }

        return null;
    }

    static double PobierzPoczatkoweSaldo()
    {
        while (true)
        {
            Console.WriteLine("Podaj początkowe saldo w zł (np. 100.50):");
            if (double.TryParse(Console.ReadLine(), out double s) && s > 0)
                return s;
            Console.WriteLine("Niepoprawne saldo.");
        }
    }

    static double PobierzKwoteZakladu(double saldo)
    {
        Console.WriteLine($"Podaj kwotę zakładu w zł (np. 12.50, max {saldo:F2}):");
        if (!double.TryParse(Console.ReadLine(), out double k)) return -1;
        if (k <= 0 || k > saldo)
        {
            Console.WriteLine("Niepoprawna kwota.");
            return -1;
        }
        return k;
    }

    static int PobierzLiczbe()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int n) && n >= 0 && n <= 36)
                return n;
            Console.WriteLine("Podaj liczbę 0-36:");
        }
    }

    static List<int> PobierzListeLiczb(int ile)
    {
        while (true)
        {
            string[] parts = Console.ReadLine().Split(' ');
            if (parts.Length == ile && parts.All(x => int.TryParse(x, out int n) && n >= 0 && n <= 36))
                return parts.Select(int.Parse).ToList();
            Console.WriteLine($"Podaj dokładnie {ile} liczb (0-36).");
        }
    }

    static string KolorLiczby(int n)
    {
        if (n == 0) return "zielony";
        if (czerwone.Contains(n)) return "czerwony";
        if (czarne.Contains(n)) return "czarny";
        return "brak";
    }

    static bool CheckDozen(int wynik, string d)
    {
        return (d == "1-12" && wynik >= 1 && wynik <= 12)
            || (d == "13-24" && wynik >= 13 && wynik <= 24)
            || (d == "25-36" && wynik >= 25 && wynik <= 36);
    }

    static (int, int) ZnajdzWspolrzedne(int n)
    {
        for (int i = 0; i < plansza.GetLength(0); i++)
            for (int j = 0; j < plansza.GetLength(1); j++)
                if (plansza[i, j] == n)
                    return (i, j);
        return (-1, -1);
    }

    static bool CzySplitPoprawny(List<int> liczby)
    {
        if (liczby.Count != 2) return false;
        var coords = liczby.Select(n => ZnajdzWspolrzedne(n)).ToList();
        int dx = Math.Abs(coords[0].Item1 - coords[1].Item1);
        int dy = Math.Abs(coords[0].Item2 - coords[1].Item2);
        return (dx + dy == 1);
    }

    static bool CzyCornerPoprawny(List<int> liczby)
    {
        if (liczby.Count != 4) return false;
        var coords = liczby.Select(n => ZnajdzWspolrzedne(n)).ToList();
        coords.Sort();
        return (coords[0].Item1 + 1 == coords[2].Item1 && coords[0].Item2 + 1 == coords[1].Item2);
    }
}
