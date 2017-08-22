using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHUD: MonoBehaviour{
    
    public string maxHP;
    public string maxMP;
    public string hpCurrentPercentage;
    public string mpCurrentPercentage;
    public string expCurrentPercentage;

    public void Start()
    {
        hpCurrentPercentage = "1";
        mpCurrentPercentage = "1";
    }

    public void CurrentHP(string message)
    {
        hpCurrentPercentage = message;

        Text percentageText = GameObject.Find("HealthPercentage").GetComponent<Text>();
        if (float.Parse(hpCurrentPercentage) <= 0.2f)
        {
            percentageText.text = "<color=#e67f84ff>" + (float.Parse(hpCurrentPercentage) * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 0, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 0, 0, 255);
        }
        else if (float.Parse(hpCurrentPercentage) <= 0.5f)
        {
            percentageText.text = "<color=#f9ca45ff>" + (float.Parse(hpCurrentPercentage) * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 174, 0, 190);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 77, 0, 255);
        }
        else
        {
            percentageText.text = "<color=#64b78eff>" + (float.Parse(hpCurrentPercentage) * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(0, 135, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(0, 77, 0, 255);
        }
        
        float maxLimitWidth = GameObject.Find("HealthMask").GetComponent<RectTransform>().sizeDelta.x;
        float currentY = GameObject.Find("HealthMask").GetComponent<RectTransform>().sizeDelta.y;
        float currentX = float.Parse(hpCurrentPercentage) * maxLimitWidth;        
        GameObject.Find("CurrentHealth").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }

    public void CurrentMP(string message)
    {
        mpCurrentPercentage = message;

        Text percentageText = GameObject.Find("ManaPercentage").GetComponent<Text>();
        percentageText.text = (float.Parse(mpCurrentPercentage) * 100).ToString("0") + "%";
        
        float maxLimitWidth = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta.x;
        float currentY = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta.y;
        float currentX = float.Parse(mpCurrentPercentage) * maxLimitWidth;
        GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }

    public void ExperienceBar(string message)
    {
        expCurrentPercentage = message;

        Text percentageText = GameObject.Find("ExpPercentage").GetComponent<Text>();
        percentageText.text = "Exp: " + (float.Parse(expCurrentPercentage) * 100).ToString("0") + "%";

        float maxLimitWidth = GameObject.Find("ExpMask").GetComponent<RectTransform>().sizeDelta.x;
        float currentY = GameObject.Find("ExpMask").GetComponent<RectTransform>().sizeDelta.y;
        float currentX = float.Parse(expCurrentPercentage) * maxLimitWidth;
        GameObject.Find("CurrentExp").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }
}
