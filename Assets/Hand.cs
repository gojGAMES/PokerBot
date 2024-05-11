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
    public List<Card> cards;
    public Dictionary<int, int> ranks; //syntax of rank as key
    public HandType HandType;

    void Sort()
    {
        
    }

    public int GetHighestRank()
    {
        int highest = 0;
        foreach (Card card in cards)
        {
            if (card.Rank > highest)
                highest = card.Rank;
        }

        return highest;
    }

    public void ResetHand()
    {
        cards.Clear();
        ranks.Clear();
        HandType = default;
    }
}
