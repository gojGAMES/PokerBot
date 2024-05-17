using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRenderer : MonoBehaviour
{
    private readonly Vector2 cardSpriteSize = new Vector2(42f, 60f);
    public Texture2D Sprites;
    public Sprite cardBack;

    public Image[] cardImages = new Image[5];
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderHand(Hand hand)
    {
        for (int i = 0; i < 5; i++)
        {
            cardImages[i].sprite = SetSprite(hand.Cards[i]);
        }
    }

    public void RenderFaceDown()
    {
        for (int i = 0; i < 5; i++)
        {
            cardImages[i].sprite = cardBack;
        }
    }
    
    public void SwapVisual(bool[] swapOrNot) 
    {
        for (int i = 0; i < 5; i++)
        {
            if (!swapOrNot[i])
            {
                cardImages[i].color = Color.white;
            }
            else
            {
                cardImages[i].color = Color.white * 0.5f;
            }
        }
    }

    public void ResetSwapVisual()
    {
        foreach (Image img in cardImages)
        {
            img.color = Color.white;
        }
    }
    
    private Sprite SetSprite(Card card)
    {
        /// x1 = 11 + rank * (909 / 14)
        /// y1 = 2 + suit * (259 / 4)
        /// x2 = x1 + 42
        /// y2 = y1 + 60
        ///
        Rect rect = new Rect(new Vector2(11f + card.Rank * (909f/14f), 2f + (float) card.Suit * (259f/4f)), cardSpriteSize);

        if (card.Rank == 13)
        {
            rect.x = 11f;
        }
        
        return Sprite.Create(Sprites, rect, new Vector2(0.5f, 0.5f));
    }
}
