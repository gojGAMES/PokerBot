using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Hand RobotHand;
    private float riskiness; //how ready the robot is to call/raise and play worse hands (or maybe playing bad hands goes more into avghand??)
    private float averageHand; //todo: rename this. this is the value the of hand the robot will strive towards
    
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
        /// throak: swap one or swap two?
        /// 2pair: swapping last one is ideal. only way that is suboptimal is if it comes to tiebreaker and the high is the deciding factor
        /// 1pair: 2nd most variable. can become 2pair, throak, foak, or house. probably best to swap 3 tho
        /// hicard: most variable. could go into anything really. if one away from flush, odds are ~9/47 (~19%). one away from straight can mean it's needed in middle, it can be affixed or suffixed. -ffixed has double the odds (3/47 vs 6/47) (~6.3% vs ~12.8%)
        /// hicard: cont'd - for a pair, best odds are swapping 3 (~38.4%). if no particularly high card is possessed, switching all may be worthwhile. that said, folding is also a good option
        ///
        
        
    }

    public Bettings Bet1()
    {
        return Bettings.call;
    }

    public Bettings Bet2()
    {
        return Bettings.call;
    }
}
