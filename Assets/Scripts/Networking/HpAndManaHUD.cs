using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpAndManaHUD {

    public float maxHP = 250;
    public float maxMP = 250;
    public float currentHP;
    public float currentMP;
    public float percentageHP;
    public float percentageMP;
    Room room;

    public HpAndManaHUD(Room room)
    {
        currentHP = maxHP;
        currentMP = maxMP;
        percentageHP = 1;
        percentageMP = 1;
        this.room = room;
    }

    public void RecieveHpHUD(string changeRate)
    {
        ChangeHP(changeRate);
    }

    public void RecieveMpHUD(string changeRate)
    {
        ChangeMP(changeRate);
    }

    public void RecieveHpAndMpHUD(string changeRate)
    {
        ChangeHP(changeRate);
        ChangeMP(changeRate);
    }

    public void ChangeHP(string deltaHP)
    {
        float valueDeltaHP = float.Parse(deltaHP);
        if (valueDeltaHP <= 0) //Loose HP
        {
            currentHP -= valueDeltaHP;
            if (currentHP <= 0)
            {
                currentHP = 0;
                room.SendMessageToAllPlayers("PlayersAreDead");
            }
        }
        else // Gain HP
        {
            currentHP += valueDeltaHP;
            if (currentHP >= maxHP)
            {
                currentHP = maxHP;
            }
            Debug.Log(currentHP);
        }
        percentageHP = currentHP / maxHP;
        room.SendMessageToAllPlayers("DisplayChangeHP/" + percentageHP);
    }

    public void ChangeMaxHP(string NewMaxHP)
    {
        float valueMaxHP = float.Parse(NewMaxHP);
        maxHP = valueMaxHP;
        ChangeHP(NewMaxHP);
    }

    public void ChangeMP(string deltaMP)
    {
        float valueDeltaMP = float.Parse(deltaMP);
        if (valueDeltaMP <= 0)//Loose MP
        {
            currentMP -= valueDeltaMP;
            if (currentMP <= 0)
            {
                currentMP = 0;
            }
        }
        else //Gain MP
        {
            currentMP += valueDeltaMP;
            if (currentMP >= maxMP)
            {
                currentMP = maxMP;
            }
            Debug.Log(currentMP);
        }
        percentageMP = currentMP / maxMP;
        room.SendMessageToAllPlayers("DisplayChangeMP/" + percentageMP);
    }

    public void ChangeMaxMP(string NewMaxMP)
    {
        float valueMaxMP = float.Parse(NewMaxMP);
        maxMP = valueMaxMP;
        ChangeMP(NewMaxMP);
    }

}
