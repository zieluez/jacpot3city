using System;
using System.Collections.Generic;
using System.Linq;

namespace CasinoCardGames
{
    class Program
    {
        static Random rand = new Random();
        static decimal balance = 0;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // dla ♠♥♦♣

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Kasyno ===");
                Console.WriteLine($"Saldo: {balance} PLN");
                Console.WriteLine("1. Wpłać pieniądze");
                Console.WriteLine("2. Wypłać pieniądze");
                Console.WriteLine("3. Blackjack");
                Console.WriteLine("4. Poker Hold'em");
                Console.WriteLine("5. Baccarat");
                Console.WriteLine("6. Wyjście");
                Console.Write("Wybierz opcję: ");
                var key = Console.ReadLine();

                switch (key)
                {
                    case "1": Deposit(); break;
                    case "2": Withdraw(); break;
                    case "3": if (balance > 0) PlayBlackjack(); else WarnNoMoney(); break;
                    case "4": if (balance > 0) PlayPoker(); else WarnNoMoney(); break;
                    case "5": if (balance > 0) PlayBaccarat(); else WarnNoMoney(); break;
                    case "6": return;
                    default: Console.WriteLine("Nieprawidłowy wybór."); Pause(); break;
                }
            }
        }

        static void WarnNoMoney()
        {
            Console.WriteLine("Brak środków na koncie. Najpierw wpłać pieniądze.");
            Pause();
        }

        static void Deposit()
        {
            Console.Write("Podaj kwotę do wpłaty: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                balance += amount;
                Console.WriteLine($"Wpłacono {amount} PLN.");
            }
            else
            {
                Console.WriteLine("Nieprawidłowa kwota.");
            }
            Pause();
        }

        static void Withdraw()
        {
            Console.Write("Podaj kwotę do wypłaty: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                if (amount <= balance)
                {
                    balance -= amount;
                    Console.WriteLine($"Wypłacono {amount} PLN.");
                }
                else
                {
                    Console.WriteLine("Nie masz tyle środków.");
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa kwota.");
            }
            Pause();
        }

        static void PlayBlackjack()
        {
            decimal bet = GetBet();
            if (bet <= 0) return;

            Deck deck = new Deck();
            deck.Shuffle();

            List<Card> playerHand = new List<Card> { deck.Draw(), deck.Draw() };
            List<Card> dealerHand = new List<Card> { deck.Draw(), deck.Draw() };

            Console.Clear();
            Console.WriteLine($"Twoja ręka:");
            DisplayCards(playerHand);

            Console.WriteLine($"Krupier pokazuje:");
            DisplayCards(new List<Card> { dealerHand[0] }); // tylko 1 karta krupiera na początku

            bool playerStands = false;
            while (!playerStands)
            {
                int playerScore = CalculateBlackjackScore(playerHand);
                if (playerScore > 21)
                {
                    Console.WriteLine("Przekroczyłeś 21, przegrywasz.");
                    balance -= bet;
                    Pause();
                    return;
                }

                Console.WriteLine("Co chcesz zrobić? (h)it, (s)tand");
                var input = Console.ReadKey(true).KeyChar;
                if (input == 'h' || input == 'H')
                {
                    var card = deck.Draw();
                    playerHand.Add(card);
                    Console.WriteLine($"Dobierasz: {card.ToAscii()}");
                    DisplayCards(playerHand);
                }
                else if (input == 's' || input == 'S')
                {
                    playerStands = true;
                }
            }

            // Krupier odsłania drugą kartę i dobiera do 17+
            Console.WriteLine("\nKrupier odsłania karty:");
            DisplayCards(dealerHand);

            while (CalculateBlackjackScore(dealerHand) < 17)
            {
                var card = deck.Draw();
                dealerHand.Add(card);
                Console.WriteLine($"Krupier dobiera: {card.ToAscii()}");
            }

            int dealerScoreFinal = CalculateBlackjackScore(dealerHand);
            int playerScoreFinal = CalculateBlackjackScore(playerHand);

            Console.WriteLine($"Twój wynik: {playerScoreFinal}");
            Console.WriteLine($"Wynik krupiera: {dealerScoreFinal}");

            if (dealerScoreFinal > 21 || playerScoreFinal > dealerScoreFinal)
            {
                Console.WriteLine("Wygrywasz!");
                balance += bet;
            }
            else if (dealerScoreFinal == playerScoreFinal)
            {
                Console.WriteLine("Remis.");
            }
            else
            {
                Console.WriteLine("Przegrywasz.");
                balance -= bet;
            }
            Pause();
        }

        static int CalculateBlackjackScore(List<Card> hand)
        {
            int score = 0;
            int aceCount = 0;
            foreach (var card in hand)
            {
                if (card.Rank >= 10) score += 10;
                else if (card.Rank == 14)
                {
                    aceCount++;
                    score += 11;
                }
                else score += card.Rank;
            }
            while (score > 21 && aceCount > 0)
            {
                score -= 10;
                aceCount--;
            }
            return score;
        }

        static void PlayPoker()
        {
            decimal bet = GetBet();
            if (bet <= 0) return;

            Deck deck = new Deck();
            deck.Shuffle();

            List<Card> playerHand = new List<Card> { deck.Draw(), deck.Draw() };
            List<Card> dealerHand = new List<Card> { deck.Draw(), deck.Draw() };
            List<Card> community = new List<Card> { deck.Draw(), deck.Draw(), deck.Draw(), deck.Draw(), deck.Draw() };

            Console.Clear();
            Console.WriteLine("Twoje karty:");
            DisplayCards(playerHand);

            Console.WriteLine("Karty krupiera:");
            DisplayCards(dealerHand);

            Console.WriteLine("Karty wspólne:");
            DisplayCards(community);

            var playerBestHand = PokerHandEvaluator.EvaluateBestHand(playerHand.Concat(community).ToList());
            var dealerBestHand = PokerHandEvaluator.EvaluateBestHand(dealerHand.Concat(community).ToList());

            Console.WriteLine($"\nTwój najlepszy układ: {playerBestHand.Name}");
            DisplayCards(playerBestHand.Cards);

            Console.WriteLine($"\nKrupier najlepszy układ: {dealerBestHand.Name}");
            DisplayCards(dealerBestHand.Cards);

            int cmp = PokerHandEvaluator.CompareHands(playerBestHand, dealerBestHand);
            if (cmp > 0)
            {
                Console.WriteLine("Wygrywasz!");
                balance += bet;
            }
            else if (cmp < 0)
            {
                Console.WriteLine("Przegrywasz.");
                balance -= bet;
            }
            else
            {
                Console.WriteLine("Remis.");
            }
            Pause();
        }

        static void PlayBaccarat()
        {
            decimal bet = GetBet();
            if (bet <= 0) return;

            Deck deck = new Deck();
            deck.Shuffle();

            List<Card> playerHand = new List<Card> { deck.Draw(), deck.Draw() };
            List<Card> bankerHand = new List<Card> { deck.Draw(), deck.Draw() };

            Console.Clear();
            Console.WriteLine("Twoje karty:");
            DisplayCards(playerHand);

            Console.WriteLine("Karty bankiera:");
            DisplayCards(bankerHand);

            int playerPoints = BaccaratPoints(playerHand);
            int bankerPoints = BaccaratPoints(bankerHand);

            Console.WriteLine($"Punkty gracza: {playerPoints}");
            Console.WriteLine($"Punkty bankiera: {bankerPoints}");

            // Reguły dobierania 3-ciej karty

            Card playerThird = null;
            Card bankerThird = null;

            bool playerDraws = playerPoints <= 5;
            if (playerDraws)
            {
                playerThird = deck.Draw();
                playerHand.Add(playerThird);
                playerPoints = BaccaratPoints(playerHand);
                Console.WriteLine($"Gracz dobiera trzecią kartę: {playerThird.ToAscii()} (punkty teraz {playerPoints})");
            }
            else
            {
                Console.WriteLine("Gracz nie dobiera trzeciej karty.");
            }

            // Bankier dobiera w zależności od kart i punktów gracza
            if (playerThird == null)
            {
                // gracz nie dobierał 3-ciej karty
                if (bankerPoints <= 5)
                {
                    bankerThird = deck.Draw();
                    bankerHand.Add(bankerThird);
                    bankerPoints = BaccaratPoints(bankerHand);
                    Console.WriteLine($"Bankier dobiera trzecią kartę: {bankerThird.ToAscii()} (punkty teraz {bankerPoints})");
                }
                else
                {
                    Console.WriteLine("Bankier nie dobiera trzeciej karty.");
                }
            }
            else
            {
                // gracz dobierał, bankier dobiera wg bardziej skomplikowanych zasad:

                int pt3rank = playerThird.Rank > 10 ? 0 : playerThird.Rank; // W Baccarat 10, J,Q,K liczymy jako 0 przy regułach bankiera

                // Bankier dobiera trzecią kartę jeśli:
                // - Bankier ma 0-2 punkty -> dobiera zawsze
                // - Bankier ma 3 punkty i gracz nie ma 8
                // - Bankier ma 4 punkty i gracz 2-7
                // - Bankier ma 5 punktów i gracz 4-7
                // - Bankier ma 6 punktów i gracz 6-7

                bool bankDraw = false;
                switch (bankerPoints)
                {
                    case 0:
                    case 1:
                    case 2:
                        bankDraw = true; break;
                    case 3:
                        if (pt3rank != 8) bankDraw = true; break;
                    case 4:
                        if (pt3rank >= 2 && pt3rank <= 7) bankDraw = true; break;
                    case 5:
                        if (pt3rank >= 4 && pt3rank <= 7) bankDraw = true; break;
                    case 6:
                        if (pt3rank == 6 || pt3rank == 7) bankDraw = true; break;
                }

                if (bankDraw)
                {
                    bankerThird = deck.Draw();
                    bankerHand.Add(bankerThird);
                    bankerPoints = BaccaratPoints(bankerHand);
                    Console.WriteLine($"Bankier dobiera trzecią kartę: {bankerThird.ToAscii()} (punkty teraz {bankerPoints})");
                }
                else
                {
                    Console.WriteLine("Bankier nie dobiera trzeciej karty.");
                }
            }

            Console.WriteLine($"\nKońcowe karty gracza:");
            DisplayCards(playerHand);
            Console.WriteLine($"Punkty gracza: {playerPoints}");

            Console.WriteLine($"Końcowe karty bankiera:");
            DisplayCards(bankerHand);
            Console.WriteLine($"Punkty bankiera: {bankerPoints}");

            // Wyłonienie zwycięzcy
            string result;
            if (playerPoints > bankerPoints)
                result = "Gracz";
            else if (bankerPoints > playerPoints)
                result = "Bankier";
            else
                result = "Remis";

            Console.WriteLine($"Wynik: {result}");

            // Wygrane
            // Zakład na gracza: 1:1
            // Zakład na bankiera: 0.95:1 (pobierana prowizja 5%)
            // Remis: zwraca stawkę

            Console.WriteLine("Na kogo stawiasz? (g) Gracz, (b) Bankier, (r) Remis");
            var choice = Console.ReadKey(true).KeyChar;

            if (choice == 'g' || choice == 'G')
            {
                if (result == "Gracz")
                {
                    Console.WriteLine("Wygrywasz zakład na Gracza!");
                    balance += bet;
                }
                else
                {
                    Console.WriteLine("Przegrywasz zakład na Gracza.");
                    balance -= bet;
                }
            }
            else if (choice == 'b' || choice == 'B')
            {
                if (result == "Bankier")
                {
                    decimal win = bet * 0.95m;
                    Console.WriteLine($"Wygrywasz zakład na Bankiera! Dostajesz {win} PLN.");
                    balance += win;
                }
                else
                {
                    Console.WriteLine("Przegrywasz zakład na Bankiera.");
                    balance -= bet;
                }
            }
            else if (choice == 'r' || choice == 'R')
            {
                if (result == "Remis")
                {
                    Console.WriteLine("Remis! Zakład zwracany.");
                }
                else
                {
                    Console.WriteLine("Przegrywasz zakład na Remis.");
                    balance -= bet;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowy wybór - zakład przepadł.");
                balance -= bet;
            }

            Pause();
        }

        static int BaccaratPoints(List<Card> hand)
        {
            int sum = 0;
            foreach (var c in hand)
            {
                int val = c.Rank > 9 ? 0 : c.Rank;
                sum += val;
            }
            return sum % 10;
        }

        static decimal GetBet()
        {
            Console.Write("Podaj stawkę: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal bet))
            {
                if (bet > 0 && bet <= balance)
                    return bet;
                else
                {
                    Console.WriteLine("Nieprawidłowa stawka lub brak wystarczających środków.");
                    Pause();
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa wartość.");
                Pause();
                return 0;
            }
        }

        static void DisplayCards(List<Card> cards)
        {
            foreach (var card in cards)
            {
                Console.ForegroundColor = card.GetConsoleColor();
                Console.Write(card.ToAscii() + " ");
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        static void Pause()
        {
            Console.WriteLine("Naciśnij dowolny klawisz
