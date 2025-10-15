using System;
using System.IO;
using System.Linq;

class Program
{
    static Random rnd = new Random();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n1) PESEL + Email 2) IBAN + Telefon 3) Wyjście");
            Console.Write("Wybierz: ");
            string opcja = Console.ReadLine();

            if (opcja == "1") Weryfikuj("PESEL", "email");
            else if (opcja == "2") Weryfikuj("IBAN", "telefon");
            else if (opcja == "3") break;
            else Console.WriteLine("Zły wybór.");
        }
    }

    static void Weryfikuj(string typ1, string typ2)
    {
        Console.Write($"Podaj {typ1}: ");
        string a = Console.ReadLine();
        Console.Write($"Podaj {typ2}: ");
        string b = Console.ReadLine();

        if (typ1.Equals("PESEL", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsPeselValid(a))
            {
                Console.WriteLine("❌ Niepoprawny PESEL.");
                return;
            }
        }
        else if (typ1.Equals("IBAN", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsIbanValid(a))
            {
                Console.WriteLine("❌ Niepoprawny IBAN (wymagany format PL + 26 cyfr).");
                return;
            }
        }

        if (typ2.Equals("telefon", StringComparison.OrdinalIgnoreCase))
        {
            if (!IsPhoneValid(b))
            {
                Console.WriteLine("❌ Niepoprawny numer telefonu (musi mieć 9 cyfr).");
                return;
            }
        }
        else
        {
            if (b.Length < 5 || !b.Contains("@"))
            {
                Console.WriteLine("❌ Niepoprawny adres e-mail.");
                return;
            }
        }

        string kod = rnd.Next(100000, 999999).ToString();
        Console.WriteLine($"(Symulacja) Kod: {kod}");
        Console.Write("Wpisz kod: ");

        if (Console.ReadLine() == kod)
        {
            File.AppendAllText("verifications.txt", $"{typ1}:{a};{typ2}:{b}\n");
            Console.WriteLine("✅ Weryfikacja OK!");
        }
        else Console.WriteLine("❌ Błędny kod.");
    }

    static bool IsPeselValid(string pesel)
    {
        if (pesel == null || pesel.Length != 11 || !pesel.All(char.IsDigit))
            return false;

        int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
        int suma = 0;

        for (int i = 0; i < 10; i++)
            suma += wagi[i] * (pesel[i] - '0');

        int kontrolna = (10 - (suma % 10)) % 10;
        return kontrolna == (pesel[10] - '0');
    }

    static bool IsIbanValid(string iban)
    {
        if (string.IsNullOrWhiteSpace(iban)) return false;
        iban = iban.Replace(" ", "").ToUpper();

        if (!iban.StartsWith("PL") || iban.Length != 28)
            return false;

        string numeryczne = iban.Substring(4) + "2521" + iban.Substring(2, 2);
        string przeniesiony = "";
        foreach (char c in numeryczne)
        {
            if (char.IsDigit(c)) przeniesiony += c;
            else przeniesiony += (c - 55).ToString();
        }

        try
        {
            int reszta = 0;
            foreach (char c in przeniesiony)
                reszta = (reszta * 10 + (c - '0')) % 97;

            return reszta == 1;
        }
        catch { return false; }
    }

    static bool IsPhoneValid(string tel)
    {
        return tel != null && tel.Length == 9 && tel.All(char.IsDigit);
    }
}
