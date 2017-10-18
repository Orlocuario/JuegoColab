using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHUD: MonoBehaviour{
    
    public float maxHP;
    public float maxMP;
    public float hpCurrentPercentage;
    public float mpCurrentPercentage;
    public float expCurrentPercentage;

    public void Start()
    {
        hpCurrentPercentage = 1f;
        mpCurrentPercentage = 1f;
    }

    public void CurrentHP(string message)
    {
        hpCurrentPercentage = float.Parse(message);

        Vector2 healthMaskSizeDelta =  GameObject.Find("HealthMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("HealthPercentage").GetComponent<Text>();
        
        if (hpCurrentPercentage <= 0.2f)
        {
            percentageText.text = "<color=#e67f84ff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 0, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 0, 0, 255);
        }
        else if (hpCurrentPercentage <= 0.5f)
        {
            percentageText.text = "<color=#f9ca45ff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 174, 0, 190);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 77, 0, 255);
        }
        else
        {
            percentageText.text = "<color=#64b78eff>" + (hpCurrentPercentage * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(0, 135, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(0, 77, 0, 255);
        }
        
        float maxLimitWidth = healthMaskSizeDelta.x;

        float currentX = hpCurrentPercentage * maxLimitWidth;      
        float currentY = healthMaskSizeDelta.y;
        
        GameObject.Find("CurrentHealth").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }

    public void CurrentMP(string message)
    {
        mpCurrentPercentage = float.Parse(message);

        Vector2 manaMaskSizeDelta = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("ManaPercentage").GetComponent<Text>();

        float maxLimitWidth = manaMaskSizeDelta.x;

        float currentX = mpCurrentPercentage * maxLimitWidth;
        float currentY = manaMaskSizeDelta.y;

        GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);

        percentageText.text = (mpCurrentPercentage * 100).ToString("0") + "%";
    }

    public void ExperienceBar(string message)
    {
        expCurrentPercentage = float.Parse(message);

        Vector2 expMaskSizeDelta = GameObject.Find("ExpMask").GetComponent<RectTransform>().sizeDelta;
        Text percentageText = GameObject.Find("ExpPercentage").GetComponent<Text>();

        float maxLimitWidth = expMaskSizeDelta.x;

        float currentX = expCurrentPercentage * maxLimitWidth;
        float currentY = expMaskSizeDelta.y;

        GameObject.Find("CurrentExp").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);

        percentageText.text = "Exp: " + (expCurrentPercentage * 100).ToString("0") + "%";
    }
}
