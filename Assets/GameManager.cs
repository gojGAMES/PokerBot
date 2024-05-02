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
    private bool[] swapCards =  {false, false, false, false, false };

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

    //todo: Maybe move this to the hand class?
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

        if (suits.Count > 1)
        {
            flush = false;
        }
        else
        {
            flush = true;
        }

        straight = ContainsStraight(hand.cards);
        if (!flush && !straight)
        {
            hand.HandType = HandType.high;
            return;
        }

        if (straight && !flush)
        {
            hand.HandType = HandType.straight;
        }

        if (!straight && flush)
        {
            hand.HandType = HandType.flush;
        }

        if (hand.GetHighestRank() == 13)
        {
            hand.HandType = HandType.royal;
        }
        else
        {
            hand.HandType = HandType.straightflush;
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

            if (iFound = false)
            {
                return false;
            }
        }

        return true;
    }

    void SwapPlayerHand()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            swapCards[0] = !swapCards[0];
        }
        if (Input.GetKey(KeyCode.W))
        {
            swapCards[1] = !swapCards[1];
        }
        if (Input.GetKey(KeyCode.E))
        {
            swapCards[2] = !swapCards[2];
        }
        if (Input.GetKey(KeyCode.R))
        {
            swapCards[3] = !swapCards[3];
        }
        if (Input.GetKey(KeyCode.T))
        {
            swapCards[4] = !swapCards[4];
        }

        if (Input.GetButtonDown("Submit"))
        {
            SwapCards(PlayerHand);
            phase++;
        }
    }

    void SwapCards(Hand hand)
    {
        for (int i = 4; i > 0; i--)
        {
            if (swapCards[i])
            {
                hand.cards.RemoveAt(i);
            }
        }

        if (hand.cards.Count == 5)
        {
            return;
        }

        GenerateHand(hand.cards);
    }

    void DetermineWinner()
    {
        if ((int)PlayerHand.HandType > (int)RobotHand.HandType)
        {
            Debug.Log("Player Wins");
        }
        if ((int)PlayerHand.HandType < (int)RobotHand.HandType)
        {
            Debug.Log("Robot Wins");
        }
        
        /// TIEBREAKERS HERE
        /// use a switch statement to run through the various tiebreaker scenarios
        /// then also add submit or smth
    }

    void ResetToNewRound()
    {
        phase = 0;
        drawnCards.Clear();
        PlayerHand.ResetHand();
        RobotHand.ResetHand();
        swapCards = default;
    }
}
