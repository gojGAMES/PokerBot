using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Bettings
{
    call,
    fold,
    raise
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private int ante = 50;
    [SerializeField] private int startingMoney = 1000;
    [SerializeField] private int playerWallet;
    [SerializeField] private int robotWallet;
    [SerializeField] private int pot;
    [SerializeField] private int phase = 0;
    private char pick;
    private int minimumBet;
    private bool playerAllIn = false;
    public Slider raiseSlider;
    public TextMeshProUGUI raiseText;

    // private List<Card> PlayerHand = new List<Card>();
    // private List<Card> RobotHand = new List<Card>();
    public Hand PlayerHand;
    public Hand RobotHand;
    public RobotController RobotController;
    public CardRenderer CardRenderer;

    private static CardEqualityComparer _cardEqualityComparer = new();
    private HashSet<Card> drawnCards = new HashSet<Card>(_cardEqualityComparer);
    [SerializeField] private bool[] swapCards =  {false, false, false, false, false };

// Start is called before the first frame update
    void Start()
    {
        playerWallet = startingMoney;
        robotWallet = startingMoney;

        minimumBet = ante;
        pick = 'ඞ';
        raiseSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerHand.PrintHand();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("The drawn Cards set contains " + drawnCards.Count + " cards: ");
            foreach (Card card in drawnCards)
            {
                Debug.Log(card.Rank + " of " + card.Suit);
            }
        }
        Game();
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
                AnteUp();
                break;
            case 1:
                GenerateHand(PlayerHand);
                CardRenderer.RenderHand(PlayerHand);
                GenerateHand(RobotHand);
                PlayerHand.AnalyzeHand();
                RobotHand.AnalyzeHand();
                Debug.Log("Hands analyzed");
                phase++;
                break;
            case 2:
                PlayerBet1();
                break;
            case 3:
                RobotBet1();
                break;
            case 4:
                SwapPlayerHand();
                CardRenderer.RenderHand(PlayerHand);
                break;
            case 5:
                PlayerHand.AnalyzeHand();
                RobotHand.AnalyzeHand();
                phase++;
                break;
            case 6:
                RobotBet1();
                break;
            case 7:
                PlayerBet1();
                break;
            
            case 8:
                DetermineWinner();
                break;
            case 9:
                ResetToNewRound();
                break;
            
        }
    }

    void AnteUp()
    {
        if (playerWallet < ante)
        {
            Debug.Log("Player broke af, robot wins");
            phase = 11;
            return;
        }

        if (robotWallet < ante)
        {
            Debug.Log("robot broke, humanity wins");
            phase = 11;
            return;
        }
        
        if (Input.GetButtonDown("Confirm"))
        {
            playerWallet -= ante;
            robotWallet -= ante;
            pot += 2 * ante;
            Debug.Log("Anted Up");
            phase++;
        }
    }

    void GenerateHand(Hand hand)
    {
        for (; hand.Cards.Count < 5; )
        {
            Card card = new Card(Random.Range(0, 3), Random.Range(1, 13));
            //Card card = Instantiate(new GameObject("card " + hand.Cards.Count, typeof(Card)), Vector3.zero, Quaternion.identity).GetComponent(Card);
            // GameObject go1 = new GameObject();
            // go1.AddComponent<Card>();
            // Card card = go1.GetComponent<Card>();
            // card = new Card(Random.Range(0,3), Random.Range(1,13))
            
            if (drawnCards.Contains(card))
            {
                Debug.LogWarning("duplicate card drawn");
                continue;
            }
            hand.Cards.Add(card);
            drawnCards.Add(card);
        }
    }
    

    void PlayerBet1()
    {
        /// we got check/call and raise (and fold)
        ///
        ///

        if (playerAllIn)
        {
            Debug.Log("player has already gone all in");
            phase++;
            return;
        }
        
        if (Input.GetButtonDown("call"))
        {
            pick = 'c';
            raiseSlider.gameObject.SetActive(false);
        }
        if (Input.GetButtonDown("raise"))
        {
            pick = 'r';
            raiseSlider.gameObject.SetActive(true);
        }
        if (Input.GetButtonDown("fold"))
        {
            pick = 'f';
            raiseSlider.gameObject.SetActive(false);
        }

        switch (pick)
        {
            case 'c':
                playerWallet -= minimumBet;
                pot += minimumBet;
                Debug.Log("bet 1 complete");
                phase++;
                return;
                break;
            case 'r':
                int sliderValue = (int)raiseSlider.value;
                raiseText.text = "Raise: " + sliderValue;
                if (sliderValue >= playerWallet)
                {
                    sliderValue = playerWallet;
                    raiseText.text = "All in!!";
                }
                
                if (Input.GetButtonDown("Confirm"))
                {
                    if (sliderValue > minimumBet)
                    {
                        if (sliderValue == playerWallet)
                        {
                            playerAllIn = true;
                        }
                        minimumBet = sliderValue;
                        playerWallet -= sliderValue;
                        pot += sliderValue;
                        phase++;
                        raiseSlider.gameObject.SetActive(false);
                        return;
                    }
                    Debug.Log("bet higher coward");
                }
                break;
                case 'f':
                    RobotWin();
                    ResetToNewRound();
                    return;
            case 'ඞ':
                return;
                break;
        }
        
        if (Input.GetButtonDown("Confirm"))
        {
            Debug.Log("bet 1 complete");
            phase++;
        }
    }

    void RobotBet1()
    {
        switch (RobotController.Bet1())
        {
            case Bettings.call:
                robotWallet -= minimumBet;
                pot += minimumBet;
                break;
        }
        phase++;
    }

    void SwapPlayerHand()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            swapCards[0] = !swapCards[0];
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            swapCards[1] = !swapCards[1];
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            swapCards[2] = !swapCards[2];
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            swapCards[3] = !swapCards[3];
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            swapCards[4] = !swapCards[4];
        }
        
        CardRenderer.SwapVisual(swapCards);

        if (Input.GetButtonDown("Submit"))
        {
            SwapCards(PlayerHand);
            CardRenderer.ResetSwapVisual();
            phase++;
        }
    }

    void SwapCards(Hand hand)
    {
        for (int i = 4; i > 0; i--)
        {
            if (swapCards[i])
            {
                hand.Cards.RemoveAt(i);
            }
        }

        if (hand.Cards.Count == 5)
        {
            return;
        }

        GenerateHand(hand);
    }

    void Bet2()
    {
        if (Input.GetButtonDown("Confirm"))
        {
            Debug.Log("Anted Up");
            phase++;
        }
    }

    void RobotBet2()
    {
        RobotController.Bet2();
        phase++;
    }

    void DetermineWinner()
    {
        phase++;
        
        if ((int)PlayerHand.HandType > (int)RobotHand.HandType)
        {
            PlayerWin();
            return;
        }
        if ((int)PlayerHand.HandType < (int)RobotHand.HandType)
        {
            RobotWin();
            return;
        }
        
        /// TIEBREAKERS HERE
        /// use a switch statement to run through the various tiebreaker scenarios
        /// then also add submit or smth

        int playerHighest = 0;
        int robotHighest = 0;

        int playerNextHighest = 0;
        int robotNextHighest = 0;
        
        switch (PlayerHand.HandType)
        {
            case HandType.high:
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Rank > playerHighest)
                    {
                        playerHighest = card.Rank;
                    }
                }
                foreach (Card card in RobotHand.Cards)
                {
                    if (card.Rank > playerHighest)
                    {
                        robotHighest = card.Rank;
                    }
                }

                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }
                
                Tie();
                break;
            
            
            case HandType.pair:
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 2)
                    {
                        playerHighest = kvp.Key;
                        break;
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 2)
                    {
                        robotHighest = kvp.Key;
                        break;
                    }
                }

                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                }

                Tie();
                break;
            
            case HandType.throak:
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 3)
                    {
                        playerHighest = kvp.Key;
                        break;
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 3)
                    {
                        robotHighest = kvp.Key;
                        break;
                    }
                }

                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                }

                Tie();
                break;
            
            case HandType.foak:
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 4)
                    {
                        playerHighest = kvp.Key;
                        break;
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 4)
                    {
                        robotHighest = kvp.Key;
                        break;
                    }
                }

                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                }

                Tie();
                break;
            
            case HandType.twopair:
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 2)
                    {
                        if (kvp.Key > playerHighest)
                        {
                            playerNextHighest = playerHighest;
                            playerHighest = kvp.Key;
                        }
                        else
                        {
                            playerNextHighest = kvp.Key;
                        }
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 2)
                    {
                        if (kvp.Key > robotHighest)
                        {
                            robotNextHighest = robotHighest;
                            robotHighest = kvp.Key;
                        }
                        else
                        {
                            robotNextHighest = kvp.Key;
                        }
                    }
                }

                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }
                
                if (playerNextHighest > robotNextHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotNextHighest > playerNextHighest)
                {
                    RobotWin();
                    break;
                }

                Tie();
                break;
            
            case HandType.house:
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 3)
                    {
                        playerHighest = kvp.Key;
                        break;
                    }
                }
                
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 3)
                    {
                        robotHighest = kvp.Key;
                        break;
                    }
                }
                
                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }

                Tie();
                break;
            
            case HandType.straight:
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Rank > playerHighest)
                        playerHighest = card.Rank;
                }
                foreach (Card card in RobotHand.Cards)
                {
                    if (card.Rank > robotHighest)
                        robotHighest = card.Rank;
                }
                
                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }

                Tie();
                break;
            
            case HandType.flush:
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Rank > playerHighest)
                        playerHighest = card.Rank;
                }
                foreach (Card card in RobotHand.Cards)
                {
                    if (card.Rank > robotHighest)
                        robotHighest = card.Rank;
                }
                
                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }

                Tie();
                break;
            
            case HandType.straightflush:
                foreach (Card card in PlayerHand.Cards)
                {
                    if (card.Rank > playerHighest)
                        playerHighest = card.Rank;
                }
                foreach (Card card in RobotHand.Cards)
                {
                    if (card.Rank > robotHighest)
                        robotHighest = card.Rank;
                }
                
                if (playerHighest > robotHighest)
                {
                    PlayerWin();
                    break;
                }

                if (robotHighest > playerHighest)
                {
                    RobotWin();
                    break;
                }

                Tie();
                break;
            case HandType.royal:
                Debug.Log("yes this is accounted for. i know this didnt happen naturally");
                Tie();
                break;
        }

        
    }

    void RobotWin()
    {
        robotWallet += pot;
    }

    void PlayerWin()
    {
        playerWallet += pot;
    }

    void Tie()
    {
        playerWallet += pot / 2;
        robotWallet += pot / 2;
    }

    void ResetToNewRound()
    {
        if (!Input.GetButtonDown("Confirm"))
        {
            return;
        }
        phase = 0;
        drawnCards.Clear();
        PlayerHand.ResetHand();
        RobotHand.ResetHand();
        for (int i = 0; i < 5; i++)
        {
            swapCards[i] = false;
        }
        pick = 'ඞ';
        playerAllIn = false;
    }
}
