using System.Collections;
using System.Collections.Generic;


public enum Suit
{
    spades, 
    clubs,
    diamonds,
    hearts
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

public class CardEqualityComparer : IEqualityComparer<Card>
{
    public bool Equals(Card c1, Card c2)
    {
        return (c1.Rank == c2.Rank && c1.Suit == c2.Suit);
    }

    public int GetHashCode(Card card) => card.Rank * (int)card.Suit;
}


