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
}
