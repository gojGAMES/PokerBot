using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Hand RobotHand;
    private float riskiness; //how ready the robot is to call/raise and play worse hands (or maybe playing bad hands goes more into avghand??)
    private float averageHand; //todo: rename this. this is the value the of hand the robot will strive towards
    
    ///additional variables
    /// hand confidence
    /// percieved player confidence
    /// loss aversion?? idk

    ///todo:
    /// robot swapping behavior
    /// robot betting 1
    /// robot betting 2
    /// robot learning
}
