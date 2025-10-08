using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Random random = new Random();
        bool graTrwa = true;
        double saldo = 100.0;

        Console.WriteLine("Witaj w grze Ruletka!");
        Console.WriteLine("Masz początkową kwotę 100.0 PLN.");
        Console.WriteLine("Możesz obstawiać na liczby (0-36), kolory (czarny/czerwony), dwunastki lub pary liczb.");

        while (graTrwa)
        {
            Console.WriteLine($"\nTwoje aktualne saldo to: {saldo} PLN");

            List<(string typZakladu, double kwota, string wartosc)> zaklady = new List<(string, double, string)>();

            while (true)
            {
                Console.WriteLine("\nWybierz typ zakładu:");
                Console.WriteLine("1 - Liczba (0-36)");
                Console.WriteLine("2 - Kolor (czarny/czerwony)");
                Console.WriteLine("3 - Dwunastka (1-12 / 13-24 / 25-36)");
                Console.WriteLine("4 - Para liczb (np. 7/8)");
                Console.WriteLine("5 - Zakończ tury");

                int wybor;
                if (!int.TryParse(Console.ReadLine(), out wybor))
                {
                    Console.WriteLine("Niepoprawny wybór, spróbuj ponownie.");
                    continue;
                }

                if (wybor == 5)
                {
                    break;
                }

                if (wybor == 1)
                {
                    Console.WriteLine("Wybierz liczbę (0-36): ");
                    if (!int.TryParse(Console.ReadLine(), out int liczba) || liczba < 0 || liczba > 36)
                    {
                        Console.WriteLine("Liczba musi być z przedziału 0-36!");
                        continue;
                    }

                    double kwotaZakladu = PobierzKwoteZakladu(saldo);
                    if (kwotaZakladu == -1) continue;

                    saldo -= kwotaZakladu;
                    zaklady.Add(("liczba", kwotaZakladu, liczba.ToString()));
                }
                else if (wybor == 2)
                {
                    Console.WriteLine("Wybierz kolor (czarny/czerwony): ");
                    string kolor = Console.ReadLine().ToLower();

                    if (kolor != "czarny" && kolor != "czerwony")
                    {
                        Console.WriteLine("Niepoprawny kolor! Wybierz czarny lub czerwony.");
                        continue;
                    }

                    double kwotaZakladu = PobierzKwoteZakladu(saldo);
                    if (kwotaZakladu == -1) continue;

                    saldo -= kwotaZakladu;
                    zaklady.Add(("kolor", kwotaZakladu, kolor));
                }
                else if (wybor == 3)
                {
                    Console.WriteLine("Wybierz dwunastkę: 1-12, 13-24 lub 25-36");
                    string dwunastka = Console.ReadLine().Trim();

                    if (dwunastka != "1-12" && dwunastka != "13-24" && dwunastka != "25-36")
                    {
                        Console.WriteLine("Niepoprawna dwunastka.");
                        continue;
                    }

                    double kwotaZakladu = PobierzKwoteZakladu(saldo);
                    if (kwotaZakladu == -1) continue;

                    saldo -= kwotaZakladu;
                    zaklady.Add(("dwunastka", kwotaZakladu, dwunastka));
                }
                else if (wybor == 4)
                {
                    Console.WriteLine("Wpisz parę liczb w formacie X/Y (np. 7/8): ");
                    string para = Console.ReadLine().Trim();

                    if (!para.Contains("/"))
                    {
                        Console.WriteLine("Niepoprawny format pary.");
                        continue;
                    }

                    string[] liczbyPara = para.Split('/');
                    if (liczbyPara.Length != 2 ||
                        !int.TryParse(liczbyPara[0], out int n1) || n1 < 0 || n1 > 36 ||
                        !int.TryParse(liczbyPara[1], out int n2) || n2 < 0 || n2 > 36)
                    {
                        Console.WriteLine("Niepoprawne liczby w parze.");
                        continue;
                    }

                    double kwotaZakladu = PobierzKwoteZakladu(saldo);
                    if (kwotaZakladu == -1) continue;

                    saldo -= kwotaZakladu;
                    zaklady.Add(("para", kwotaZakladu, para));
                }
                else
                {
                    Console.WriteLine("Niepoprawny wybór, spróbuj ponownie.");
                }
            }

            int wynikRuletki = random.Next(0, 37);
            string kolorWyniku = WybierzKolor(wynikRuletki);

            Console.WriteLine($"\nWylosowana liczba to: {wynikRuletki} ({kolorWyniku})");

            foreach (var zaklad in zaklady)
            {
                string typZakladu = zaklad.typZakladu;
                double kwota = zaklad.kwota;
                string wartosc = zaklad.wartosc;

                if (typZakladu == "liczba")
                {
                    if (int.Parse(wartosc) == wynikRuletki)
                    {
                        Console.WriteLine($"Wygrałeś zakład na liczbę {wartosc}!");
                        saldo += kwota * 35;
                    }
                    else
                    {
                        Console.WriteLine($"Przegrałeś zakład na liczbę {wartosc}.");
                    }
                }
                else if (typZakladu == "kolor")
                {
                    if (wartosc == kolorWyniku)
                    {
                        Console.WriteLine($"Wygrałeś zakład na kolor {wartosc}!");
                        saldo += kwota * 2;
                    }
                    else
                    {
                        Console.WriteLine($"Przegrałeś zakład na kolor {wartosc}.");
                    }
                }
                else if (typZakladu == "dwunastka")
                {
                    bool wygrana = false;

                    if (wartosc == "1-12" && wynikRuletki >= 1 && wynikRuletki <= 12)
                        wygrana = true;
                    else if (wartosc == "13-24" && wynikRuletki >= 13 && wynikRuletki <= 24)
                        wygrana = true;
                    else if (wartosc == "25-36" && wynikRuletki >= 25 && wynikRuletki <= 36)
                        wygrana = true;

                    if (wygrana)
                    {
                        Console.WriteLine($"Wygrałeś zakład na dwunastkę {wartosc}!");
                        saldo += kwota * 3;
                    }
                    else
                    {
                        Console.WriteLine($"Przegrałeś zakład na dwunastkę {wartosc}.");
                    }
                }
                else if (typZakladu == "para")
                {
                    string[] liczbyPara = wartosc.Split('/');
                    int n1 = int.Parse(liczbyPara[0]);
                    int n2 = int.Parse(liczbyPara[1]);

                    if (wynikRuletki == n1 || wynikRuletki == n2)
                    {
                        Console.WriteLine($"Wygrałeś zakład na parę liczb {wartosc}!");
                        saldo += kwota * 17; // Split payout to 17:1
                    }
                    else
                    {
                        Console.WriteLine($"Przegrałeś zakład na parę liczb {wartosc}.");
                    }
                }
            }

            if (saldo <= 0)
            {
                Console.WriteLine("\nNiestety, Twoje saldo zostało wyczerpane. Gra kończy się.");
                graTrwa = false;
            }
        }
    }

    static double PobierzKwoteZakladu(double saldo)
    {
        Console.WriteLine("Podaj kwotę zakładu (np. 30zł): ");
        string wejscie = Console.ReadLine();

        string liczbaStr = new string(wejscie.Where(c => Char.IsDigit(c) || c == '.').ToArray());

        if (!double.TryParse(liczbaStr, out double kwotaZakladu))
        {
            Console.WriteLine("Niepoprawna kwota. Wprowadź liczbę.");
            return -1;
        }

        if (kwotaZakladu > saldo)
        {
            Console.WriteLine("Nie masz wystarczającej ilości środków na ten zakład.");
            return -1;
        }

        return kwotaZakladu;
    }

    static string WybierzKolor(int liczba)
    {
        string[] czarne = { "2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35" };
        string[] czerwone = { "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36" };

        if (liczba == 0)
            return "zielony";

        string liczbaStr = liczba.ToString();
        if (Array.Exists(czarne, element => element == liczbaStr))
            return "czarny";
        else if (Array.Exists(czerwone, element => element == liczbaStr))
            return "czerwony";
        else
            return "nieznany";
    }
}
