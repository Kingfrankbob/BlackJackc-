/*
    Blackjack Game made by Michael Cragun, Took 1.5 hours.
    Handles all cases that should be possible and has been tested enough! 
    Can run in console with `dotnet run` when in this directory
*/

namespace BlackJack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(@"Welcome to BlackJack! If your here you should already know how to play!
            if not please look up the rules on how to play... 
            To Clarify Aces are automatic, so you do now have a choice on that, based from there everything is automatic...
            To exit just type exit while playing");

            var random = new Random();
            var winStreak = 0;
            bool firstime = true;

            Console.WriteLine("Game Starting");


            while (true)
            {

                Game game = new Game();
                game.Setup();

                bool bust = false;

                while (!bust)
                {
                    if (!firstime) Console.Clear();
                    else
                    {
                        firstime = false;
                        Console.WriteLine();
                    }

                    Console.WriteLine("Dealer's Cards");
                    PrintCardsHoriz(game.dealer, true);

                    Console.WriteLine("Player's Cards");
                    PrintCardsHoriz(game.player1);

                    Console.WriteLine("Would you like to hit or stay? (h/s)");

                    var input = Console.ReadLine() ?? "";
                    if (input.ToLower() == "s") break;
                    if (input.ToLower() == "h")
                    {
                        var card = game.GetNextCard();
                        game.player1.Add(card);

                        if (game.player1.Sum(card => card.valueI) > 21)
                        {
                            game.TryConvertAces(1);
                            if (game.player1.Sum(card => card.valueI) > 21)
                            {
                                bust = true;
                                break;
                            }
                        }
                    }
                }

                if (bust)
                {
                    PrintStats(game);
                    Console.WriteLine("You busted and lost...");
                    HandleWinStreak(true, winStreak);
                    winStreak = 0;
                }
                else
                {
                    while (game.dealer.Sum(card => card.valueI) < 17)
                    {
                        var card = game.GetNextCard();
                        game.dealer.Add(card);
                    }
                    if (game.player1.Sum(card => card.valueI) > game.dealer.Sum(card => card.valueI)
                     || game.dealer.Sum(card => card.valueI) > 21)
                    {
                        PrintStats(game);
                        Console.WriteLine("Congratulations, you beat the dealer!");
                        winStreak++;
                        HandleWinStreak(false, winStreak);

                    }
                    else if (game.player1.Sum(card => card.valueI) == game.dealer.Sum(card => card.valueI))
                    {
                        PrintStats(game);
                        Console.WriteLine("You tied with the dealer! No one wins!");
                    }
                    else
                    {
                        PrintStats(game);
                        Console.WriteLine("Sorry, you lost to the dealer!");
                        HandleWinStreak(true, winStreak);
                        winStreak = 0;
                    }
                }

                Console.WriteLine("Press Enter to play again or type exit to exit");
                var awaitNextGame = Console.ReadLine() ?? "";
                if (awaitNextGame == "exit") { HandleWinStreak(false, winStreak, true); break; }

            }
        }


        public static void PrintStats(Game game)
        {
            Console.Clear();

            Console.WriteLine("Dealer's Cards");
            PrintCardsHoriz(game.dealer);
            Console.WriteLine("Player's Cards");
            PrintCardsHoriz(game.player1);

            Console.WriteLine(@"Dealer's Total {0}! Player's Total {1}!",
             game.dealer.Sum(card => card.valueI),
             game.player1.Sum(card => card.valueI));
        }
        public static void HandleWinStreak(bool breakStreak, int winStreak, bool exiting = false)
        {
            if (exiting)
            {
                if (winStreak > 2)
                    Console.WriteLine("You ended with a win streak of " + winStreak + "! Good Job!");
                Console.WriteLine("Thanks for playing! Come Back Soon!");
                return;
            }
            if (breakStreak)
            {
                if (winStreak > 2)
                    Console.WriteLine("You broke your win streak of" + winStreak + "! Too Bad!");
                else
                    Console.WriteLine("You broke your win streak! Better luck next time!");
            }
            else if (winStreak % 5 == 0)
            {
                Console.WriteLine("You are on a win streak of" + winStreak + "! Keep it up!");
            }

        }
        public static void PrintCardsHoriz(List<Cards> cards, bool isDealer = false)
        {
            var cardHeight = cards[0].cardMatrix.GetLength(0);
            bool Censor = true;

            for (int i = 0; i < cardHeight; i++)
            {
                foreach (var card in cards)
                {
                    for (int j = 0; j < card.cardMatrix.GetLength(1); j++)
                    {
                        if (isDealer && i == 2 && j == 2 && Censor)
                        {
                            Console.Write("X");
                            Censor = false;
                            continue;
                        }
                        Console.Write(card.cardMatrix[i, j]);
                    }
                    Console.Write("  "); // Add spacing between cards
                }
                Console.WriteLine(); // Move to the next row
            }
        }
    }


    public class Game
    {
        public List<Cards> deck = new List<Cards>();
        public List<Cards> player1 = new List<Cards>();
        public List<Cards> player2 = new List<Cards>();
        public List<Cards> player3 = new List<Cards>();
        public List<Cards> player4 = new List<Cards>();
        public List<Cards> dealer = new List<Cards>();

        public static char[] Cards = new char[] { 'A', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'J', 'Q', 'K' };
        public static Random random = new Random();
        public void Setup()
        {
            while (true)
            {
                bool playerr = true;
                bool breakk = false;
                for (int i = 0; i < 4; i++)
                {
                    if (i == 3)
                    {
                        breakk = true;
                    }
                    while (true)
                    {
                        var card = new Cards(Cards[random.Next(0, 13)]);
                        if (card.isFaceCard)
                        {
                            if (deck.Where(cardd => cardd.valueF == card.valueF).Count() < 4)
                            {
                                deck.Add(card);

                                if (playerr)
                                    player1.Add(card);
                                else
                                    dealer.Add(card);

                                break;
                            }
                        }
                        else if (card.isAce && deck.Where(cardd => cardd.isAce).Count() < 4)
                        {
                            if (playerr)
                            {
                                var currentTotal = player1.Sum(cardd => cardd.valueI);
                                if (currentTotal + 11 > 21)
                                {
                                    card.valueI = 1;
                                    deck.Add(card);
                                    player1.Add(card);
                                    break;
                                }
                                else
                                {
                                    card.valueI = 11;
                                    deck.Add(card);
                                    player1.Add(card);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (deck.Where(cardd => cardd.valueI == card.valueI).Count() < 4
                             && card.valueI != 10)
                            {
                                deck.Add(card);

                                if (playerr)
                                    player1.Add(card);
                                else
                                    dealer.Add(card);

                                break;
                            }
                            if (deck.Where(cardd => cardd.valueI == card.valueI).Count() < 16
                             && card.valueI == 10
                              && !card.isFaceCard)
                            {
                                deck.Add(card);

                                if (playerr)
                                    player1.Add(card);
                                else
                                    dealer.Add(card);

                                break;
                            }
                        }
                    }

                    if (playerr)
                        playerr = false;
                    else
                        playerr = true;

                }
                if (breakk) break;
            }
        }

        public Cards GetNextCard()
        {
            bool breakk = false;

            Cards returnCard = new(Cards[random.Next(0, 13)]);

            while (true)
            {
                var card = new Cards(Cards[random.Next(0, 13)]);
                if (card.isFaceCard)
                {
                    if (deck.Where(cardd => cardd.valueF == card.valueF).Count() < 4)
                    {
                        deck.Add(card);
                        returnCard = card;
                        break;
                    }
                }
                else if (card.isAce && deck.Where(cardd => cardd.isAce).Count() < 4)
                {
                    var currentTotal = player1.Sum(cardd => cardd.valueI);
                    if (currentTotal + 11 > 21)
                    {
                        card.valueI = 1;
                        deck.Add(card);
                        returnCard = card;
                        break;
                    }
                    else
                    {
                        card.valueI = 11;
                        deck.Add(card);
                        returnCard = card;
                        break;
                    }
                }
                else
                {
                    if (deck.Where(cardd => cardd.valueI == card.valueI).Count() < 4
                     && card.valueI != 10)
                    {
                        deck.Add(card);
                        returnCard = card;
                        break;
                    }
                    if (deck.Where(cardd => cardd.valueI == card.valueI).Count() < 16
                     && card.valueI == 10
                      && !card.isFaceCard)
                    {
                        deck.Add(card);
                        returnCard = card;
                        break;
                    }
                }
                if (breakk) break;
            }

            return returnCard;
        }

        public void TryConvertAces(int player)
        {
            if (player == 1)
            {
                foreach (Cards card in player1)
                {
                    if (card.isAce)
                    {
                        card.valueI = 1;
                    }
                }
            }
        }

    }



    public class Cards
    {
        public char[,] cardMatrix = new char[5, 5] {
            {'-', '-', '-', '-', '-' },
            {'|', ' ', ' ', ' ', '|' },
            {'|', ' ', 'X', ' ', '|' },
            {'|', ' ', ' ', ' ', '|' },
            {'-', '-', '-', '-', '-' } };
        public bool isFaceCard = false;
        public bool isAce;
        public char valueF;
        public int valueI;
        public Cards(char value)
        {
            bool pass = false;
            this.valueF = value;
            if (value == 'J' || value == 'Q' || value == 'K')
            {
                isFaceCard = true;
                isAce = false;
            }
            if (value == 'A')
            {
                isAce = true;
                isFaceCard = false;
            }
            else if (isFaceCard)
            {
                valueI = 10;
            }
            else
            {
                valueF = ' ';
                int ValueT;

                if ((int)char.GetNumericValue(value) == 0)
                {
                    ValueT = 10;
                    cardMatrix[2, 2] = 'T';
                    pass = true;
                }
                else
                    ValueT = (int)char.GetNumericValue(value);

                valueI = ValueT;
            }
            if (!pass) cardMatrix[2, 2] = value;
        }
    }
}