using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Hand RobotHand;
    private float riskiness; //how ready the robot is to call/raise and play worse hands (or maybe playing bad hands goes more into avghand??)
    private float averageHand; //todo: rename this. this is the value the of hand the robot will strive towards
    private float percievedPlayerHandValue; //should factor the average hand value along with confidence gleaned from player betting
    /// player high bluff rate - odds that the player is playing a bad hand
    /// player low bluff rate - odds that the player is hiding a good hand
    /// predicted player hand value - can be determined based on avg hand value and swapped cards
    /// player call rate - rate at which player calls or raises when robot plays aggressive (in other words how likely the player is to assume bluffs)
    ///  
    
    ///additional variables
    /// hand confidence
    /// percieved player confidence
    /// loss aversion?? idk

    ///todo:
    /// robot swapping behavior
    /// robot betting 1
    /// robot betting 2
    /// robot learning
    /// robot hand analysis
    ///

    
    ///notes:
    /// -only swapped hands should contribute to averagePlayerHandValue
    public void RobotSwap()
    {
        /// hand type can be used to gague hand value
        /// top 5 require no swaps (maybe 6 even?)
        /// throak: swap one or swap two? - can become foak or house
        /// 2pair: swapping last one is ideal. only way that is suboptimal is if it comes to tiebreaker and the high is the deciding factor
        /// 1pair: 2nd most variable. can become 2pair, throak, foak, or house. probably best to swap 3 tho
        /// hicard: most variable. could go into anything really. if one away from flush, odds are ~9/47 (~19%). one away from straight can mean it's needed in middle, it can be affixed or suffixed. -ffixed has double the odds (3/47 vs 6/47) (~6.3% vs ~12.8%)
        /// hicard cont'd - for a pair, best odds are swapping 3 (~38.4%). if no particularly high card is possessed, switching all may be worthwhile. that said, folding is also a good option
        ///
        
        
        switch (RobotHand.HandType)
        {
            default:
                break;
            case HandType.twopair:
                //find the outlier, and swap it
                int outlierRank = 0;
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 1)
                    {
                        outlierRank = kvp.Key;
                        break;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (RobotHand.Cards[i].Rank == outlierRank)
                    {
                        RobotHand.swapCards[i] = true;
                    }
                }
                break;
            
            case HandType.throak:
                //swap both non-throaks (8.5% -> 10.6%)
                int throakRank = 0;
                foreach (KeyValuePair<int, int> kvp in RobotHand.ranks)
                {
                    if (kvp.Value == 3)
                    {
                        throakRank = kvp.Key;
                        break;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (RobotHand.Cards[i].Rank != throakRank)
                    {
                        RobotHand.swapCards[i] = true;
                    }
                }
                break;
            
            case HandType.pair:
                //TODO: add more strategy
                //this swaps all non pair cards
                for (int i = 0; i < 5; i++)
                {
                    if (RobotHand.ranks[RobotHand.Cards[i].Rank] == 1)
                    {
                        RobotHand.swapCards[i] = true;
                    }
                }
                break;
            
            case HandType.high:
                //a few different strategies: play for straight (6.3 || 12.8), play for flush (19), play for straight and flush, or play for pair 
                ///first, determine strategy to use based on percieved risk vs percieved reward
                /// 

                switch (DetermineHighCardPlayType())
                {
                    case 3:
                        OneFromFlush(out int straightFlushIndex);
                        RobotHand.swapCards[straightFlushIndex] = true;
                        break;
                    case 2:
                        OneFromStraight(out int straightIndex);
                        RobotHand.swapCards[straightIndex] = true;
                        break;
                    case 1:
                        OneFromFlush(out int flushIndex);
                        RobotHand.swapCards[flushIndex] = true;
                        break;
                    case 0:
                        int secondHighestRank = 0;
                        int highestRank = RobotHand.GetHighestRank();
                        for (int j = 0; j < 5; j++)
                        {
                            if (RobotHand.Cards[j].Rank < highestRank && RobotHand.Cards[j].Rank > secondHighestRank)
                            {
                                secondHighestRank = RobotHand.Cards[j].Rank;
                            }
                        }

                        for (int j = 0; j < 5; j++)
                        {
                            if (RobotHand.Cards[j].Rank < secondHighestRank)
                            {
                                RobotHand.swapCards[j] = true;
                            }
                        }
                        break;
                }
                break;
        }
    }

    int DetermineHighCardPlayType()
    {
        //TODO: flesh this out

        int oneFromStraight = OneFromStraight(out int straightOutlierIndex);
        int oneFromFlush = OneFromFlush(out int flushOutlierIndex);
        
        //return 0 to play for pairs
        //return 1 to play for flush
        // 2 to play for straight
        // 3 to play for both
        if (oneFromStraight > 0 && oneFromFlush == 1)
        {
            if (straightOutlierIndex == flushOutlierIndex)
            {
                return 3;
            }
        }

        float playStraightAppeal = ((float)HandType.straight - percievedPlayerHandValue) * (0.063f * oneFromStraight);
        float playFlushAppeal = ((float) HandType.flush - percievedPlayerHandValue) * (0.19f * oneFromFlush); //todo: more accurately reflect the odds
        float playPairAppeal = ((float) HandType.pair - percievedPlayerHandValue) * 0.384f;

        if (playPairAppeal > playFlushAppeal && playPairAppeal > playStraightAppeal)
        {
            return 0;
        }

        if (playFlushAppeal > playPairAppeal && playFlushAppeal > playStraightAppeal)
        {
            return 1;
        }
        
        if (playStraightAppeal > playFlushAppeal && playStraightAppeal > playPairAppeal)
        {
            return 2;
        }
        
        return 0;
    }

    public Bettings Bet1()
    {
        return Bettings.call;
    }

    public Bettings Bet2()
    {
        return Bettings.call;
    }


    int OneFromStraight(out int OutlierIndex)
    {
        //if no, return 0. if yes, return 1 if only one desirable result, and 2 for two (affix/suffix)

        int lowestRank = 13;
        bool found = false;

        OutlierIndex = 6;

        foreach (Card card in RobotHand.Cards)
        {
            if (card.Rank < lowestRank)
            {
                lowestRank = card.Rank;
            }
        }

        List<int> straightIndexes = new List<int>();

        int outliers = 0;
        int inARow = 1;
        
        for (int j = lowestRank; j < lowestRank + 5; j++)
        {
            //TODO: use modulo 13 to check for wrapping straights
            found = false;
            for (int k = 0; k < 5; k++)
            {
                if (RobotHand.Cards[k].Rank == j)
                {
                    found = true;
                    straightIndexes.Add(k);
                    if (outliers == 0)
                        inARow++;
                    break;
                }
            }
            
            if (!found)
            {
                outliers++;
                if (outliers == 2)
                {
                    return 0;
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (straightIndexes.Contains(i))
                continue;

            OutlierIndex = i;
            
        }

        if (lowestRank > 1)
        {
            if (inARow == 4)
            {
                return 2;
            }
        }
        
        return 1;
    }

    int OneFromFlush(out int outlierIndex)
    {
        outlierIndex = 6;
        if (RobotHand.suits.Count > 2)
        {
            return 0;
        }

        Suit candidateSuit = Suit.clubs;

        foreach (KeyValuePair<Suit,int> kvp in RobotHand.suits)
        {
            if (kvp.Value != 4 || kvp.Value != 1)
            {
                return 0;
            }

            if (kvp.Value == 4)
            {
                candidateSuit = kvp.Key;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (RobotHand.Cards[i].Suit != candidateSuit)
            {
                outlierIndex = i;
                break;
            }
        }
        
        return 1;
    }
}
