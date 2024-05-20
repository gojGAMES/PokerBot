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
    public Dictionary<Suit, int> suits = new Dictionary<Suit, int>();
    public HandType HandType;
    public bool[] swapCards =  {false, false, false, false, false };

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
        for (int i = 0; i < 5; i++)
        {
            swapCards[i] = false;
        }
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
        
        ranks.Clear();
        suits.Clear();
        
        foreach (Card card in Cards)
        {
            if (!ranks.TryAdd(card.Rank, 1))
                ranks[card.Rank]++;
            if (!suits.TryAdd(card.Suit, 1))
                suits[card.Suit]++;
        }
        
        Debug.Log(gameObject.name + " contains " + ranks.Count + "different ranks of cards");

        if (ranks.Count < 5)
        {
            switch (ranks.Count)
            {
                case 4:
                    HandType = HandType.pair;
                    break;
                case 3:
                    //TODO: figure out why this doesn't work (i got a throak off the hand [8 j j 10 10] 
                    if (ranks.ContainsValue(3))
                    {
                        HandType = HandType.throak;
                    }
                    else
                    {
                        HandType = HandType.twopair;
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
            return;
        }

        if (!straight && flush)
        {
            HandType = HandType.flush;
            return;
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
        //TODO: fix this (got a straight flush with the hand [k 7 2 9 5] - all spades)
        //FIXED: added return clauses to the analysis so it doesnt run all the way through
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

    public string HandTypeName()
    {
        switch (HandType)
        {
            case HandType.high:
                return "High Card";
            case HandType.pair:
                return "Pair";
            case HandType.twopair:
                return "Two Pair";
            case HandType.throak:
                return "Three of a Kind";
            case HandType.straight:
                return "Straight";
            case HandType.flush:
                return "Flush";
            case HandType.house:
                return "Full House";
            case HandType.foak:
                return "Four of a Kind";
            case HandType.straightflush:
                return "Straight Flush";
            case HandType.royal:
                return "Godly, ~1 / 650 000 Royal Flush";
        }

        return "n/a";
    }

    public float GetHandValue()
    {
        float handValue = (float)HandType;
        int rankValue = 0;

        switch (HandType)
        {
            case HandType.foak:
                foreach (var kvp in ranks)
                {
                    if (kvp.Value == 4)
                    {
                        rankValue = kvp.Key;
                        break;
                    }
                }
                break;
            case HandType.house:
                foreach (var kvp in ranks)
                {
                    if (kvp.Value == 3)
                    {
                        rankValue = kvp.Key;
                        break;
                    }
                }
                break;
            case HandType.throak:
                foreach (var kvp in ranks)
                {
                    if (kvp.Value == 3)
                    {
                        rankValue = kvp.Key;
                        break;
                    }
                }
                break;
            case HandType.twopair:
                foreach (var kvp in ranks)
                {
                    if (kvp.Value == 2)
                    {
                        if (kvp.Key > rankValue)
                            rankValue = kvp.Key;
                        break;
                    }
                }
                break;
            case HandType.royal:
                return 14.0f;
            case HandType.straightflush:
                rankValue = GetHighestRank();
                break;
            case HandType.straight:
                rankValue = GetHighestRank();
                break;
            case HandType.flush:
                rankValue = GetHighestRank();
                break;
            case HandType.high:
                rankValue = GetHighestRank();
                break;
        }

        return handValue + (float)rankValue / 13;
    }
}
