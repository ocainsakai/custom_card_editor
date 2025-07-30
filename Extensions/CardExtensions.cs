using System;
using System.Collections.Generic;
using System.Linq;

public static class CardExtensions
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    // Utility function to get rank groups, useful for many checks
    private static List<IGrouping<CardRank, CardData>> GetRankGroups(IEnumerable<CardData> cards)
    {
        return cards.GroupBy(c => c.Rank).OrderByDescending(g => g.Count()).ToList();
    }

    // Utility function to get CardCardSuit groups, useful for flush checks
    private static List<IGrouping<CardSuit, CardData>> GetCardSuitGroups(IEnumerable<CardData> cards)
    {
        return cards.GroupBy(c => c.Suit).ToList();
    }

    public static PokerHandMatch HasPair(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var pairGroup = groups.FirstOrDefault(g => g.Count() == 2);
        if (pairGroup != null)
        {
            return new PokerHandMatch(PokerHandType.Pair, pairGroup.ToList());
        }
        return PokerHandMatch.None;
    }

    public static PokerHandMatch HasOnlyPair(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var pairs = groups.Where(g => g.Count() == 2).ToList();
        if (pairs.Count == 1)
        {
            return new PokerHandMatch(PokerHandType.Pair, pairs.First().ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasTwoPair(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var pairs = groups.Where(g => g.Count() == 2).OrderByDescending(g => g.Key).Take(2).ToList(); 
        if (pairs.Count == 2)
        {
            return new PokerHandMatch(PokerHandType.TwoPair, pairs.SelectMany(g => g).ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasThreeOfAKind(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var threeOfAKindGroup = groups.FirstOrDefault(g => g.Count() == 3);
        if (threeOfAKindGroup != null)
        {
            return new PokerHandMatch(PokerHandType.ThreeOfAKind, threeOfAKindGroup.ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasStraight(this IEnumerable<CardData> cards)
    {
        var distinctRanks = cards.Select(c => (int)c.Rank).Distinct().OrderBy(r => r).ToList();

        if (distinctRanks.Contains((int)CardRank.Ace))
        {
            distinctRanks.Insert(0, 1);
        }

        for (int i = 0; i <= distinctRanks.Count - 5; i++)
        {
            var potentialStraightRanks = distinctRanks.Skip(i).Take(5).ToList();
            bool isConsecutive = true;
            for (int j = 0; j < 4; j++)
            {
                if (potentialStraightRanks[j] + 1 != potentialStraightRanks[j + 1])
                {
                    isConsecutive = false;
                    break;
                }
            }

            if (isConsecutive)
            {
              
                var matchedCards = cards.Where(c => potentialStraightRanks.Contains((int)c.Rank)).ToList();
              
                return new PokerHandMatch(PokerHandType.Straight, matchedCards.Take(5).ToList());
            }
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasFlush(this IEnumerable<CardData> cards)
    {
        var CardSuitGroups = GetCardSuitGroups(cards);
        var flushGroup = CardSuitGroups.FirstOrDefault(g => g.Count() >= 5); 
        if (flushGroup != null)
        {
            return new PokerHandMatch(PokerHandType.Flush, flushGroup.ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasFullHouse(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var threeOfAKindGroup = groups.FirstOrDefault(g => g.Count() == 3);
        var pairGroup = groups.FirstOrDefault(g => g.Count() == 2);

        if (threeOfAKindGroup != null && pairGroup != null)
        {
            var matchedCards = threeOfAKindGroup.ToList();
            matchedCards.AddRange(pairGroup.ToList());
            return new PokerHandMatch(PokerHandType.FullHouse, matchedCards);
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasFourOfAKind(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var fourOfAKindGroup = groups.FirstOrDefault(g => g.Count() == 4);
        if (fourOfAKindGroup != null)
        {
            return new PokerHandMatch(PokerHandType.FourOfAKind, fourOfAKindGroup.ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasFiveOfAKind(this IEnumerable<CardData> cards)
    {
        var groups = GetRankGroups(cards);
        var fiveOfAKindGroup = groups.FirstOrDefault(g => g.Count() == 5);
        if (fiveOfAKindGroup != null)
        {
            return new PokerHandMatch(PokerHandType.FiveOfAKind, fiveOfAKindGroup.ToList());
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasStraightFlush(this IEnumerable<CardData> cards)
    {
        var straightMatch = HasStraight(cards);
        var flushMatch = HasFlush(cards);
        if (straightMatch != PokerHandMatch.None && flushMatch != PokerHandMatch.None)
        {
            var list = straightMatch.MatchedCards.Intersect(flushMatch.MatchedCards);
            return new PokerHandMatch(PokerHandType.StraightFlush, list);
        }
        return PokerHandMatch.None;
    }

    public static PokerHandMatch HasRoyalFlush(this IEnumerable<CardData> cards)
    {
        var CardSuitGroups = GetCardSuitGroups(cards);
        foreach (var CardSuitGroup in CardSuitGroups.Where(g => g.Count() >= 5))
        {
            var ranksInGroup = CardSuitGroup.Select(c => (int)c.Rank).Distinct().ToList();
            if (ranksInGroup.Contains((int)CardRank.Ten) &&
                ranksInGroup.Contains((int)CardRank.Jack) &&
                ranksInGroup.Contains((int)CardRank.Queen) &&
                ranksInGroup.Contains((int)CardRank.King) &&
                ranksInGroup.Contains((int)CardRank.Ace))
            {
                var royalFlushCards = CardSuitGroup.Where(c =>
                    c.Rank == CardRank.Ten || c.Rank == CardRank.Jack || c.Rank == CardRank.Queen || c.Rank == CardRank.King || c.Rank == CardRank.Ace
                ).ToList();
                return new PokerHandMatch(PokerHandType.StraightFlush, royalFlushCards);
            }
        }
        return PokerHandMatch.None;
    }



    public static PokerHandMatch HasFlushHouse(this IEnumerable<CardData> cards)
    {
        var fullHouseMatch = cards.HasFullHouse();
        var flushMatch = cards.HasFlush();
        if (fullHouseMatch != PokerHandMatch.None && flushMatch != PokerHandMatch.None)
        {
            var matchedCards = fullHouseMatch.MatchedCards.Intersect(flushMatch.MatchedCards);
            return new PokerHandMatch(PokerHandType.FlushFive, matchedCards);
        }
        return PokerHandMatch.None;
    }


    public static PokerHandMatch HasFlushFive(this IEnumerable<CardData> cards)
    {
        var fiveOfAKindMatch = cards.HasFiveOfAKind();
        var flushMatch = cards.HasFlush();
        if (fiveOfAKindMatch != PokerHandMatch.None && flushMatch != PokerHandMatch.None)
        {
             var matchedCards = fiveOfAKindMatch.MatchedCards.Intersect(flushMatch.MatchedCards);
            return new PokerHandMatch(PokerHandType.FlushFive, matchedCards);
        }
        return PokerHandMatch.None;
    }
}


public class PokerHandMatch
{
    public PokerHandType HandType { get; }
    public List<CardData> MatchedCards { get; } 

    public PokerHandMatch(PokerHandType type, IEnumerable<CardData> cards)
    {
        HandType = type;
        MatchedCards = cards?.ToList() ?? null;
    }
    public static PokerHandMatch None = new PokerHandMatch(PokerHandType.PokerType, null);
    public override string ToString()
    {
        return $"{HandType}: [{string.Join(", ", MatchedCards.Select(c => c.ToString()))}]";
    }
}