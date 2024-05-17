using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public int RaiseMultipliFactor = 10;
    
     public Slider raiseSlider;
     public TextMeshProUGUI raiseText;
     public TextMeshProUGUI callText;
     public TextMeshProUGUI PotText;
     public TextMeshProUGUI EventBubble;
     public TextMeshProUGUI PlayerWallet;
     public TextMeshProUGUI RobotWallet;

     private Vector3 sliderBasePosition;
     public GameObject BettingControlsParent;

    // Start is called before the first frame update
    void Start()
    {
        sliderBasePosition = raiseSlider.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RaiseSliderOnOff(bool onOff)
    {
        raiseSlider.gameObject.SetActive(onOff);
        
        /*if (onOff == true)
        {
            raiseSlider.transform.position = sliderBasePosition;
        }
        else
        {
            raiseSlider.transform.position = new Vector3(99999, 99999, -99999);
        }*/
    }

    public void UpdateCallText(int minBet)
    {
        callText.text = "A - Call ($" + minBet + ")";
    }

    public int GetSliderValAsInt()
    {
        return (int)raiseSlider.value * RaiseMultipliFactor;
    }

    public void UpdateRaiseText(int value, bool allIn = false)
    {
        if (allIn)
        {
            raiseText.text = "All in!!";
            return;
        }
        
        raiseText.text = "Raise: $" + value;
    }

    public void UpdatePot(int value)
    {
        PotText.text = "Pot: $" + value;
    }

    public void UpdatePlayerWallet(int value)
    {
        PlayerWallet.text = "Player: $" + value;
    }
    public void UpdateRobotWallet(int value)
    {
        RobotWallet.text = "Robot: $" + value;
    }
    

    public void DisplayEventBubble(string content)
    {
        EventBubble.text = content;
    }

    public void ToggleBettingUI(bool val)
    {
        //Note: FOR SOME REASON THIS BREAKS THE SLIDER THAT IS UNRELATED
        //callText.gameObject.transform.parent.gameObject.SetActive(val);
        BettingControlsParent.SetActive(val);
    }
}
