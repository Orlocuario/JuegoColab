using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHUD: MonoBehaviour{

    public static DisplayHUD instance;

    public void Start()
    {
        instance = this;
    }

    public void CurrentHP(string hpCurrentPercentage)
    {
        Text percentageText = GameObject.Find("HealthPercentage").GetComponent<Text>();
        percentageText.text = (float.Parse(hpCurrentPercentage)*100).ToString()+"%";

        Vector2 currentHealth = GameObject.Find("CurrentHealth").GetComponent<RectTransform>().sizeDelta;
        Vector2 limitHealth = GameObject.Find("HealthMask").GetComponent<RectTransform>().sizeDelta;
        float maxLimitWidth = limitHealth.x;
        float currentX = float.Parse(hpCurrentPercentage) * maxLimitWidth;
        float currentY = limitHealth.y;
        GameObject.Find("CurrentHealth").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }

    public void CurrentMP(string mpCurrentPercentage)
    {
        Text percentageText = GameObject.Find("ManaPercentage").GetComponent<Text>();
        percentageText.text = (float.Parse(mpCurrentPercentage) * 100).ToString() + "%";

        Vector2 currentMana = GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta;
        Vector2 limitMana = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta;
        float maxLimitWidth = limitMana.x;
        float currentX = float.Parse(mpCurrentPercentage) * maxLimitWidth;
        float currentY = limitMana.y;
        GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }
}
