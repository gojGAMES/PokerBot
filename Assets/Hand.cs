using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType
{
    high,
    pair,
    twopair,
    throak,
    straight,
    flush,
    house,
    foak,
    straightflush,
    royal
}

public class Hand : MonoBehaviour
{
    public List<Card> Cards = new List<Card>();
    public Dictionary<int, int> ranks = new Dictionary<int, int>(); //syntax of rank as key
    public HandType HandType;

    void Sort()
    {
        
    }

    public int GetHighestRank()
    {
        int highest = 0;
        foreach (Card card in Cards)
        {
            if (card.Rank > highest)
                highest = card.Rank;
        }

        return highest;
    }

    public void ResetHand()
    {
        Cards.Clear();
        ranks.Clear();
        HandType = default;
    }

    public void PrintHand()
    {
        foreach (Card card in Cards)
        {
            Debug.Log(card.Rank+" of " + card.Suit);
        }
    }
    
    public void AnalyzeHand()
    {
        bool flush;
        bool straight;
        
        Dictionary<Suit, int> suits = new Dictionary<Suit, int>();
        
        foreach (Card card in Cards)
        {
            if (ranks.TryAdd(card.Rank, 1))
                ranks[card.Rank]++;
            if (!suits.TryAdd(card.Suit, 1))
                suits[card.Suit]++;
        }

        if (ranks.Count < 5)
        {
            switch (ranks.Count)
            {
                case 4:
                    HandType = HandType.pair;
                    break;
                case 3:
                    if (ranks.ContainsValue(1))
                    {
                        HandType = HandType.twopair;
                    }
                    else
                    {
                        HandType = HandType.throak;
                    }
                    break;
                case 2:
                    if (ranks.ContainsValue(1))
                    {
                        HandType = HandType.foak;
                    }
                    else
                    {
                        HandType = HandType.house;
                    }
                    break;
            }
            return;
        }

        if (suits.Count > 1)
        {
            flush = false;
        }
        else
        {
            flush = true;
        }

        straight = ContainsStraight(Cards);
        if (!flush && !straight)
        {
            HandType = HandType.high;
            return;
        }

        if (straight && !flush)
        {
            HandType = HandType.straight;
        }

        if (!straight && flush)
        {
            HandType = HandType.flush;
        }

        if (GetHighestRank() == 13)
        {
            HandType = HandType.royal;
        }
        else
        {
            HandType = HandType.straightflush;
        }

    }
    
    bool ContainsStraight(List<Card> cards)
    {
        int lowestRank = 13;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].Rank < lowestRank)
            {
                lowestRank = cards[i].Rank;
            }
        }

        for (int i = lowestRank; i < lowestRank + cards.Count; i++)
        {
            bool iFound = false;
            for (int j = 0; j < cards.Count; j++)
            {
                if (i == cards[j].Rank)
                {
                    iFound = true;
                    break;
                }
            }

            if (iFound == false)
            {
                return false;
            }
        }

        return true;
    }
}
