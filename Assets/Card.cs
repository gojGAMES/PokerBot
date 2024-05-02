using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Suit
{
    spades,
    clubs,
    hearts,
    diamond
}

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
public class Card
{
    public Suit Suit;
    public int Rank;

    public Card(int suit, int rank)
    {
        Suit = (Suit) suit;
        Rank = rank;
    }
}

public class Hand
{
    public List<Card> cards;
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
        HandType = default;
    }
}
