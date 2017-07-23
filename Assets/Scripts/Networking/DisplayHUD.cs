using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHUD: MonoBehaviour{

    public static DisplayHUD instance;
    public string maxHP;
    public string maxMP;
    public string currentHP;
    public string currentMP;

    public void Start()
    {
        instance = this;
        /*if (currentHP != null && maxHP != null)
        {
            string percentageHP = (float.Parse(currentHP) / float.Parse(maxHP)).ToString();
            CurrentHP(percentageHP);
        }
        else
        {
            CurrentHP("1");
        }

        if (currentMP != null && maxMP != null)
        {
            string percentageMP = (float.Parse(currentMP) / float.Parse(maxMP)).ToString();
            CurrentHP(percentageMP);
        }
        else
        {
            CurrentMP("1");
        }*/ 
    }

    public void CurrentHP(string message)
    {
        char[] separator = new char[1];
        separator[0] = ':';
        string[] arreglo = message.Split(separator);

        string hpCurrentPercentage = arreglo[0];
        if (arreglo.Length != 1)
        {
            currentHP = arreglo[1];
            maxHP = arreglo[2];
        }

        Text percentageText = GameObject.Find("HealthPercentage").GetComponent<Text>();
        if (float.Parse(hpCurrentPercentage) <= float.Parse("0.2"))
        {
            percentageText.text = "<color=#e67f84ff>" + (float.Parse(hpCurrentPercentage) * 100).ToString("0") + "%" + "</color>";
            GameObject.Find("CurrentHealth").GetComponent<Image>().color = new Color32(174, 0, 0, 255);
            GameObject.Find("HealthMask").GetComponent<Image>().color = new Color32(77, 0, 0, 255);
        }
        else if (float.Parse(hpCurrentPercentage) <= float.Parse("0.5"))
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
        char[] separator = new char[1];
        separator[0] = ':';
        string[] arreglo = message.Split(separator);

        string mpCurrentPercentage = arreglo[0];
        if (arreglo.Length != 1)
        {
            currentMP = arreglo[1];
            maxMP = arreglo[2];
        }

        Text percentageText = GameObject.Find("ManaPercentage").GetComponent<Text>();
        percentageText.text = (float.Parse(mpCurrentPercentage) * 100).ToString("0") + "%";
        
        float maxLimitWidth = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta.x;
        float currentY = GameObject.Find("ManaMask").GetComponent<RectTransform>().sizeDelta.y;
        float currentX = float.Parse(mpCurrentPercentage) * maxLimitWidth;
        GameObject.Find("CurrentMana").GetComponent<RectTransform>().sizeDelta = new Vector2(currentX, currentY);
    }
}
