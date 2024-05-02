using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int ante = 50;

    private int phase = 0;

    // private List<Card> PlayerHand = new List<Card>();
    // private List<Card> RobotHand = new List<Card>();
    public Hand PlayerHand = new Hand();
    public Hand RobotHand = new Hand();

    private HashSet<Card> drawnCards = new HashSet<Card>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Game()
    {
        /// ante up <
        /// generate player hand
        /// generate robot hand
        /// analyze player hand
        /// analyze robot hand
        /// player bet <
        /// player react* <
        /// robot bet
        /// robot react*
        /// player swap cards <
        /// robot swap cards
        /// analyze player hand
        /// analyze robot hand
        /// player bet <
        /// player react* <
        /// robot bet
        /// robot react*
        /// determine winner
        /// return
        switch (phase)
        {
            case 0:
                break;
        }
    }

    void AnteUp()
    {
        if (Input.GetButton("Confirm"))
        {
            Debug.Log("Anted Up");
            phase++;
        }
    }

    void GenerateHand(List<Card> hand)
    {
        for (; hand.Count < 5; )
        {
            Card card = new Card(Random.Range(0, 3), Random.Range(1, 13));
            if (drawnCards.Contains(card))
            {
                continue;
            }
            hand.Add(card);
            drawnCards.Add(card);
        }
    }

    void AnalyzeHand(Hand hand)
    {
        bool flush;
        bool straight;
        Dictionary<int, int> ranks = new Dictionary<int, int>();
        Dictionary<Suit, int> suits = new Dictionary<Suit, int>();
        
        foreach (Card card in hand.cards)
        {
            if (!ranks.TryAdd(card.Rank, 1))
                ranks[card.Rank]++;
            if (!suits.TryAdd(card.Suit, 1))
                suits[card.Suit]++;
        }

        if (ranks.Count < 5)
        {
            switch (ranks.Count)
            {
                case 4:
                    hand.HandType = HandType.pair;
                    break;
                case 3:
                    if (ranks.ContainsValue(1))
                    {
                        hand.HandType = HandType.twopair;
                    }
                    else
                    {
                        hand.HandType = HandType.throak;
                    }
                    break;
                case 2:
                    if (ranks.ContainsValue(1))
                    {
                        hand.HandType = HandType.foak;
                    }
                    else
                    {
                        hand.HandType = HandType.house;
                    }
                    break;
            }
            return;
        }
        
    }
    
    
}
