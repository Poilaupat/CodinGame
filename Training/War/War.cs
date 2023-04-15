namespace Training.War
{
    using Console = CodinGameEmulator.Console;

    using System;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    /**
     * Auto-generated code below aims at helping you parse
     * the standard input according to the problem statement.
     **/
    class Solution
    {
        public static void Main(string[] args)
        {
            Deck d1 = new Deck();
            Deck d2 = new Deck();

            int n = int.Parse(Console.ReadLine()); // the number of cards for player 1
            for (int i = 0; i < n; i++) // the n cards of player 1
            {
                d1.Enqueue(Console.ReadLine());
            }
            int m = int.Parse(Console.ReadLine()); // the number of cards for player 2
            for (int i = 0; i < m; i++)
            {
                d2.Enqueue(Console.ReadLine());
            }

            var game = new Game(d1, d2);
            game.Run();

            if (game.OnGoingWar)
                Console.WriteLine("PAT");
            else if (game.D1.Count > game.D2.Count)
                Console.WriteLine($"1 {game.RoundCounter}");
            else
                Console.WriteLine($"2 {game.RoundCounter}");
        }
    }

    internal class Game
    {
        public int RoundCounter {get;set;} = 0;
        public bool OnGoingWar {get;set;} = false;
        public Deck D1 {get;set;}
        public Deck D2 {get;set;}
        public Deck D1Discard {get;set;} = new Deck();
        public Deck D2Discard {get;set;} = new Deck();

        public Game(Deck d1, Deck d2)
        {
            D1 = d1;
            D2 = d2;
        }

        public void Run()
        {
           while (D1.Count > 0 && D2.Count > 0)
            {
                var c1 = D1.Dequeue();
                var c2 = D2.Dequeue();

                if (c1 > c2)
                {
                    ResolveRound(D1, c1, c2);
                }
                else if (c2 > c1)
                {
                    ResolveRound(D2, c1, c2);
                }
                else
                {
                    D1Discard.Enqueue(c1);
                    D1.TransfertTo(D1Discard, 3);

                    D2Discard.Enqueue(c2);
                    D2.TransfertTo(D2Discard, 3);

                    OnGoingWar = true;
                }
            } 
        }

        private void ResolveRound(Deck winner, int c1, int c2)
        {
            D1Discard.TransfertAllTo(winner);
            winner.Enqueue(c1);
            D2Discard.TransfertAllTo(winner);
            winner.Enqueue(c2);

            RoundCounter++;
            OnGoingWar = false;
        }
    }

    internal class Deck
    {
        private Queue<int> _stack = new Queue<int>();

        public int Count
        {
            get { return _stack.Count(); }
        }

        public void Enqueue(string sCard)
        {
            int card = ParseCard(sCard);
            _stack.Enqueue(card);
        }

        public void Enqueue(int card)
        {
            _stack.Enqueue(card);
        }

        public int Dequeue()
        {
            return _stack.Dequeue();
        }

        public void TransfertAllTo(Deck deck)
        {
            while(this.Count > 0)
            {
                var card = this.Dequeue();
                deck.Enqueue(card);
            }
        }

        public void TransfertTo(Deck deck, int cardCount)
        {
            for(int i = 0; i < cardCount; i++)
            {
                if(this.Count == 0)
                    return;
                
                var card = this.Dequeue();
                deck.Enqueue(card);
            }
        }

        private int ParseCard(string sCard)
        {
            string value = sCard.Substring(0, sCard.Length - 1);

            if (int.TryParse(value, out int card))
            {
                return card;
            }
            else
            {
                return value switch
                {
                    "J" => 11,
                    "Q" => 12,
                    "K" => 13,
                    "A" => 14,
                    _ => throw new ArgumentException("Unexpected card")
                };
            }
        }
    }
}