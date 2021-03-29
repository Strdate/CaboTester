using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaboTester
{
    class CaboGame
    {
        static byte[] CardBytes = new byte[] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6,
            7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 11, 11, 11, 11, 12, 12, 12, 12, 13, 13 };

        int playerCount;

        Player[] players;

        Card[] deck;

        Card discard;

        public CaboGame(int playerCount)
        {
            this.playerCount = playerCount;
            players = new Player[playerCount];
            deck = CardBytes.Select(x => new Card(x)).ToArray();
            discard = DrawCard();
            for(int i = 0; i < players.Length; i++) {
                players[i] = new Player(this, i);
            }
        }

        public Tuple<int,int> SimulateQuickCabo()
        {
            //PrintState();
            for(int i = 1; i < players.Length; i++) {
                players[i].PlayAgainstCabo(players[0]);
            }
            //PrintState();
            return Tuple.Create( players[0].firstRoundSeenSum, PenaltyForCaller(0) );
        }

        public int PenaltyForCaller(int caller)
        {
            int[] sums = new int[playerCount];
            for(int i = 0; i < playerCount; i++) {
                sums[i] = players[i].SumCards();
            }

            if(sums.Min() == sums[caller]) {
                return 0;
            } else {
                return sums[caller] + 10;
            }
        }

        public Card DrawCard()
        {
            return DrawCards(1)[0];
        }

        public Card[] DrawCards(int count)
        {
            return Util.SelectCards(ref deck, count);
        }

        public void PrintState()
        {
            foreach(Player p in players) {
                Console.WriteLine($"Player {p.index}: " + Util.CardsToString(p.hand) + $" (sum {p.SumCards()})");
            }
        }
    }

    class Player
    {
        public int index;

        CaboGame game;

        public Card[] hand;
        public int firstRoundSeenSum;
        public int penalty;

        public Player(CaboGame game, int index)
        {
            this.game = game;
            this.index = index;
            hand = game.DrawCards(4);
            hand[0].isSeen = true;
            hand[1].isSeen = true;
            firstRoundSeenSum = hand[0].value + hand[1].value;
        }

        public void PlayAgainstCabo(Player enemy)
        {
            Card card = game.DrawCard();
            //Console.WriteLine($"Player {index} draws {card.value}");
            if(card.isSwap) {
                Card cardToSwap = PickHighCard();
                Card enemyCard = enemy.PickSeenCard();
                //Console.WriteLine($"Player {index} swaps his card {cardToSwap.value} with enemy's card {enemyCard.value}");
                cardToSwap.isSeen = false;
                enemyCard.isSeen = false;
                AddToHand(enemyCard);
                enemy.AddToHand(cardToSwap);
                return;
            }

            ExchangeHighCard(card);
        }

        public Card ExchangeHighCard(Card newCard)
        {
            newCard.isSeen = true;
            Card[] ordered = hand.OrderBy(x => x.isSeen ? x.value : 6.5).ToArray();
            Card cardToSwap = ordered.Last();
            if ((newCard.value >= cardToSwap.value && cardToSwap.isSeen) || (newCard.value > 6 && !cardToSwap.isSeen)) {
                //Console.WriteLine($"Player {index} discards card {newCard.value} with no effect");
                return newCard;
            }
            hand = ordered.Take(ordered.Length - 1).Concat( new Card[] { newCard }).ToArray();
            //Console.WriteLine($"Player {index} swaps his card {newCard.value} with his card {ordered.Last().value}");
            return ordered.Last();
        }

        public Card PickHighCard()
        {
            Card[] ordered = hand.OrderBy(x => x.isSeen ? x.value : 6.5).ToArray();
            hand = ordered.Take(ordered.Length - 1).ToArray();
            return ordered.Last();
        }

        public Card PickSeenCard()
        {
            Card[] ordered = hand.OrderBy(x => x.isSeen ? 0 : 1).ToArray();
            hand = ordered.Skip(1).ToArray();
            return ordered[0];
        }

        public void AddToHand(Card card)
        {
            hand = hand.Concat(new Card[] { card }).ToArray();
        }

        public int SumCards()
        {
            return hand.Select(x => (int)x.value).Sum();
        }
    }

    struct Card
    {
        public byte value;
        public bool isSeen;

        public Card(byte value)
        {
            this.value = value;
            isSeen = false;
        }

        public bool isSwap => value == 11 || value == 12;
    }
}
