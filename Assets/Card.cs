using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Suit
{
    spades, 
    clubs,
    diamond,
    hearts
}

public class Card : MonoBehaviour
{
    private readonly Vector2 cardSpriteSize = new Vector2(42f, 60f);
    
    public Suit Suit;
    public int Rank;
    public SpriteRenderer SpriteRenderer;
    public Texture2D Sprites;

    public Card(int suit, int rank)
    {
        Suit = (Suit) suit;
        Rank = rank;
    }

    public void SetSprite()
    {
        /// x1 = 11 + rank * (909 / 14)
        /// y1 = 2 + suit * (259 / 4)
        /// x2 = x1 + 42
        /// y2 = y1 + 60
        ///
        Rect rect = new Rect(new Vector2(11f + Rank * (909f/14f), 2f + (float) Suit * (259f/4f)), cardSpriteSize);

        if (Rank == 13)
        {
            rect.x = 11f;
        }
        
        SpriteRenderer.sprite = Sprite.Create(Sprites, rect, new Vector2(0.5f, 0.5f));
    }
}


