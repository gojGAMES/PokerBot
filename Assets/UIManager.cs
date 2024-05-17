using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
     public Slider raiseSlider;
     public TextMeshProUGUI raiseText;
     public TextMeshProUGUI callText;
     public TextMeshProUGUI PotText;
     public TextMeshProUGUI EventBubble;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RaiseSliderOnOff(bool onOff)
    {
        raiseSlider.gameObject.SetActive(onOff);
    }

    public void UpdateCallText(int minBet)
    {
        callText.text = "A - Call ($" + minBet + ")";
    }

    public int GetSliderValAsInt()
    {
        return (int)raiseSlider.value;
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

    public void DisplayEventBubble(string content)
    {
        EventBubble.text = content;
    }
}
