using System.Collections;
using System.Collections.Generic;


public enum Suit
{
    spades, 
    clubs,
    diamond,
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


