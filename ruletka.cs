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
        Console.WriteLine("Możesz obstawiać na liczby (0-36) lub na kolory (czarny/czerwony).");

        while (graTrwa)
        {
            Console.WriteLine($"\nTwoje aktualne saldo to: {saldo} PLN");

            List<(string typZakladu, double kwota, string wartosc)> zaklady = new List<(string, double, string)>();

            while (true)
            {
                Console.WriteLine("\nWybierz typ zakładu (1 - Liczba, 2 - Kolor, 3 - Zakończ tury): ");
                int wybor = int.Parse(Console.ReadLine());

                if (wybor == 3)
                {
                    break;
                }

                if (wybor == 1)
                {
                    Console.WriteLine("Wybierz liczbę (0-36): ");
                    int liczba = int.Parse(Console.ReadLine());

                    if (liczba < 0 || liczba > 36)
                    {
                        Console.WriteLine("Liczba musi być z przedziału 0-36!");
                        continue;
                    }

                    Console.WriteLine("Podaj kwotę zakładu (np. 30zł): ");
                    string wejscie = Console.ReadLine();

                    string liczbaStr = new string(wejscie.Where(c => Char.IsDigit(c) || c == '.').ToArray());

                    double kwotaZakladu;
                    if (double.TryParse(liczbaStr, out kwotaZakladu))
                    {
                        if (kwotaZakladu > saldo)
                        {
                            Console.WriteLine("Nie masz wystarczającej ilości środków na ten zakład.");
                            continue;
                        }

                        saldo -= kwotaZakladu; 
                        zaklady.Add(("liczba", kwotaZakladu, liczba.ToString()));
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawna kwota. Wprowadź liczbę.");
                    }
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

                    Console.WriteLine("Podaj kwotę zakładu (np. 30zł): ");
                    string wejscie = Console.ReadLine();

                    string liczbaStr = new string(wejscie.Where(c => Char.IsDigit(c) || c == '.').ToArray());

                    double kwotaZakladu;
                    if (double.TryParse(liczbaStr, out kwotaZakladu))
                    {
                        if (kwotaZakladu > saldo)
                        {
                            Console.WriteLine("Nie masz wystarczającej ilości środków na ten zakład.");
                            continue;
                        }

                        saldo -= kwotaZakladu; 
                        zaklady.Add(("kolor", kwotaZakladu, kolor));
                    }
                    else
                    {
                        Console.WriteLine("Niepoprawna kwota. Wprowadź liczbę.");
                    }
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
            }

            if (saldo <= 0)
            {
                Console.WriteLine("\nNiestety, Twoje saldo zostało wyczerpane. Gra kończy się.");
                graTrwa = false;
            }
        }
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