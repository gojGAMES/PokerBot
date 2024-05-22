using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    private InfoPackage infoPackage = new InfoPackage();
    
    private char pick;
    private int minimumBet;
    private bool playerAllIn = false;
    private bool robotAllIn = false;
    
    // private List<Card> PlayerHand = new List<Card>();
    // private List<Card> RobotHand = new List<Card>();
    public Hand PlayerHand;
    public Hand RobotHand;
    public RobotController RobotController;
    public CardRenderer PlayerCardRenderer;
    public CardRenderer RobotCardRenderer;
    public UIManager UIManager;
    public GameObject PauseScreen;
    private bool paused = false;
    [SerializeField]private bool debugMode = false;

    private static CardEqualityComparer _cardEqualityComparer = new();
    private HashSet<Card> drawnCards = new HashSet<Card>(_cardEqualityComparer);

// Start is called before the first frame update
    void Start()
    {
        playerWallet = startingMoney;
        robotWallet = startingMoney;

        minimumBet = ante;
        pick = 'ඞ';
        UIManager.RaiseSliderOnOff(false);
        UIManager.ToggleBettingUI(false);
        UIManager.DisplayEventBubble("Press enter to Ante up! ($"+ante+")");
        PauseScreen.SetActive(false);
        infoPackage.PlayerHand = PlayerHand;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
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
        }*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            PauseScreen.SetActive(!PauseScreen.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            debugMode = !debugMode;
        }
        
        if (paused)
            return;
        
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
            case -2:
                ResetToNewRound();
                break;
            case -1:
                if (Input.GetButtonDown("Confirm"))
                {
                    ResetToNewMatch();
                }
                break;
            case 0:
                AnteUp();
                break;
            case 1:
                GenerateHand(PlayerHand);
                PlayerCardRenderer.RenderHand(PlayerHand);
                GenerateHand(RobotHand);
                RobotCardRenderer.RenderFaceDown();
                if (debugMode)
                    RobotCardRenderer.RenderHand(RobotHand);
                PlayerHand.AnalyzeHand();
                RobotHand.AnalyzeHand();
                Debug.Log("Hands analyzed");
                phase++;
                UIManager.ToggleBettingUI(true);
                break;
            case 2:
                PlayerBet1();
                break;
            case 3:
                RobotBet1();
                break;
            case 4:
                if (Input.GetButtonDown("Confirm"))
                {
                    UIManager.DisplayEventBubble("You may now swap up to 5 cards. Use the QWERT keys to select which cards you wish to trade.");
                    phase++;
                }
                break;
            case 5:
                SwapPlayerHand();
                PlayerCardRenderer.RenderHand(PlayerHand);
                break;
            case 6:
                Debug.Log("enter phase 5");
                RobotController.RobotSwap();
                SwapCards(RobotHand);
                if (debugMode)
                    RobotCardRenderer.RenderHand(RobotHand);
                phase++;
                break;
            case 7:
                PlayerHand.AnalyzeHand();
                RobotHand.AnalyzeHand();
                phase++;
                break;
            case 8:
                RobotBet2();
                break;
            case 9:
                PlayerBet1();
                break;
            case 10:
                DetermineWinner();
                break;
            case 11:
                ResetToNewRound();
                break;
            
        }
    }

    void AnteUp()
    {
        if (playerWallet < ante)
        {
            Debug.Log("Player broke af, robot wins");
            UIManager.DisplayEventBubble("You have gone bust, and therefore the Robot wins!");
            UIManager.UIOnGameEnd();
            phase = -1;
            return;
        }

        if (robotWallet < ante)
        {
            Debug.Log("robot broke, humanity wins");
            UIManager.DisplayEventBubble("The Robot has gone bust, and therefore you win!");
            UIManager.UIOnGameEnd();
            phase = -1;
            return;
        }
        
        if (Input.GetButtonDown("Confirm"))
        {
            playerWallet -= ante;
            robotWallet -= ante;
            infoPackage.SunkCost += ante;
            AddToPot(ante*2);
            UIManager.DisplayEventBubble("The first round of betting commences. You can call, raise, or fold.");
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
        
        UIManager.UpdateCallText(minimumBet);
        
        if (playerAllIn)
        {
            Debug.Log("player has already gone all in");
            phase++;
            return;
        }
        
        if (Input.GetButtonDown("call"))
        {
            pick = 'c';
            UIManager.RaiseSliderOnOff(false);
            UIManager.ToggleBettingUI(false);
        }
        if (Input.GetButtonDown("raise"))
        {
            pick = 'r';
            UIManager.RaiseSliderOnOff(true);
        }
        if (Input.GetButtonDown("fold"))
        {
            pick = 'f';
            UIManager.RaiseSliderOnOff(false);
            UIManager.ToggleBettingUI(false);
        }

        switch (pick)
        {
            case 'c':
                playerWallet -= minimumBet;
                AddToPot(minimumBet);
                Debug.Log("bet 1 complete");
                phase++;
                pick = 'ඞ';
                return;
                break;
            case 'r':
                int sliderValue = UIManager.GetSliderValAsInt();
                UIManager.UpdateRaiseText(sliderValue, sliderValue >= playerWallet);
                
                if (Input.GetButtonDown("Confirm"))
                {
                    if (minimumBet + sliderValue >= playerWallet)
                    {
                        playerAllIn = true;
                        minimumBet = playerWallet;
                    }
                    else
                        minimumBet += sliderValue;
                    playerWallet -= minimumBet;
                    AddToPot(minimumBet);
                    infoPackage.playerRaised = true;
                    phase++;
                    pick = 'ඞ';
                    UIManager.RaiseSliderOnOff(false);
                    UIManager.ToggleBettingUI(false);
                    return;
                    
                    //UIManager.DisplayEventBubble("The raise is lower than the minimum bet. Either call, fold, or increase your raise.");
                    //Debug.Log("bet higher coward");
                }
                break;
                case 'f':
                    RobotCardRenderer.RenderHand(RobotHand);
                    infoPackage.Folded = true;
                    RobotWin();
                    phase = -2;
                    return;
            case 'ඞ':
                return;
                break;
        }
    }

    void RobotBet1()
    {
        infoPackage.MinimumBet = minimumBet;
        infoPackage.Pot = pot;
        
        switch (RobotController.Bet1(infoPackage, out int raise))
        {
            case Bettings.call:
                robotWallet -= minimumBet;
                AddToPot(minimumBet);
                infoPackage.SunkCost += minimumBet;
                UIManager.DisplayEventBubble("The Robot calls the bet.");
                break;
            case Bettings.fold:
                RobotCardRenderer.RenderHand(RobotHand);
                infoPackage.Folded = true;
                PlayerWin();
                phase = -2;
                return;
            case Bettings.raise:
                if (minimumBet + raise > robotWallet)
                {
                    minimumBet = robotWallet;
                    robotAllIn = true;
                    UIManager.DisplayEventBubble("The robot goes all in!");
                }
                else
                {
                    minimumBet += raise;
                    UIManager.DisplayEventBubble("The robot raises by $" + raise);
                }
                robotWallet -= minimumBet;
                AddToPot(minimumBet);
                break;
        }
        phase++;
    }

    void SwapPlayerHand()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerHand.swapCards[0] = !PlayerHand.swapCards[0];
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayerHand.swapCards[1] = !PlayerHand.swapCards[1];
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerHand.swapCards[2] = !PlayerHand.swapCards[2];
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerHand.swapCards[3] = !PlayerHand.swapCards[3];
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerHand.swapCards[4] = !PlayerHand.swapCards[4];
        }
        
        PlayerCardRenderer.SwapVisual(PlayerHand.swapCards);

        if (Input.GetButtonDown("Confirm"))
        {
            SwapCards(PlayerHand);
            PlayerCardRenderer.ResetSwapVisual();
            UIManager.DisplayEventBubble("The second round of betting will now commence.");
            UIManager.ToggleBettingUI(true);
            Debug.Log("inb4 phase 5");
            phase++;
            Debug.Log("phase++ to 5");
        }
    }

    void SwapCards(Hand hand)
    {
        for (int i = 4; i > 0; i--)
        {
            if (hand.swapCards[i])
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
        if (robotAllIn)
        {
            UIManager.DisplayEventBubble("The robot has already gone all in");
            phase++;
            return;
        }
        
        /*infoPackage.MinimumBet = minimumBet;
        infoPackage.Pot = pot;
        
        switch (RobotController.Bet2())
        {
            case Bettings.call:
                robotWallet -= minimumBet;
                AddToPot(minimumBet);
                break;
        }
        phase++;*/
        infoPackage.MinimumBet = minimumBet;
        infoPackage.Pot = pot;
        
        switch (RobotController.Bet2(infoPackage, out int raise))
        {
            case Bettings.call:
                robotWallet -= minimumBet;
                AddToPot(minimumBet);
                infoPackage.SunkCost += minimumBet;
                UIManager.DisplayEventBubble("The Robot calls the bet.");
                break;
            case Bettings.fold:
                infoPackage.Folded = true;
                RobotCardRenderer.RenderHand(RobotHand);
                PlayerWin();
                phase = -2;
                return;
            case Bettings.raise:
                if (minimumBet + raise > robotWallet)
                {
                    minimumBet = robotWallet;
                    robotAllIn = true;
                    UIManager.DisplayEventBubble("The robot goes all in!");
                }
                else
                {
                    minimumBet += raise;
                    UIManager.DisplayEventBubble("The robot raises by $" + raise);
                }
                robotWallet -= minimumBet;
                AddToPot(minimumBet);
                break;
        }
        phase++;
    }

    void DetermineWinner()
    {
        phase++;
        UIManager.ToggleBettingUI(false);
        RobotCardRenderer.RenderHand(RobotHand);
        
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
                        if (kvp.Key > playerHighest)
                            playerHighest = kvp.Key;
                        break;
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 2)
                    {
                        if (kvp.Key > robotHighest)
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

                playerHighest = 0;
                robotHighest = 0;
                
                foreach (KeyValuePair<int, int> kvp in PlayerHand.ranks)
                {
                    if (kvp.Value == 1)
                    {
                        if (kvp.Key > playerHighest)
                        {
                            playerHighest = kvp.Key;
                        }
                    }
                }
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 1)
                    {
                        if (kvp.Key > robotHighest)
                        {
                            robotHighest = kvp.Key;
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

    void AddToPot(int value)
    {
        pot += value;
        OnTransaction();
    }

    void OnTransaction()
    {
        UIManager.UpdatePot(pot);
        UIManager.UpdatePlayerWallet(playerWallet);
        UIManager.UpdateRobotWallet(robotWallet);
    }

    void RobotWin()
    {
        UIManager.DisplayEventBubble("The Robot wins the round with a " + RobotHand.HandTypeName()+", earning the full pot of $"+pot);
        robotWallet += pot;
        infoPackage.RobotLost = false;
        RobotController.PostRoundAnalysis(infoPackage);
    }

    void PlayerWin()
    {
        UIManager.DisplayEventBubble("The Player wins the round with a " + PlayerHand.HandTypeName()+", earning the full pot of $"+pot);
        infoPackage.RobotLost = true;
        playerWallet += pot;
        RobotController.PostRoundAnalysis(infoPackage);
    }

    void Tie()
    {
        UIManager.DisplayEventBubble("It's a tie! Both participants will split the pot evenly");
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
        UIManager.DisplayEventBubble("Press enter to Ante up! ($"+ante+")");
        
        drawnCards.Clear();
        PlayerHand.ResetHand();
        RobotHand.ResetHand();
        pick = 'ඞ';
        playerAllIn = false;
        robotAllIn = false;
        minimumBet = ante;
        pot = 0;
        OnTransaction();
    }

    void ResetToNewMatch()
    {
        playerWallet = startingMoney;
        robotWallet = startingMoney;

        minimumBet = ante;
        pick = 'ඞ';
        UIManager.RaiseSliderOnOff(false);
        UIManager.ToggleBettingUI(false);
        UIManager.UIOnNewMatch();
        
        ResetToNewRound();
    }
}
