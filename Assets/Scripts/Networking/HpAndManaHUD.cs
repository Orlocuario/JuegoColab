using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpAndManaHUD {

    public float maxHP = 250;
    public float maxMP = 250;
    public float currentHP;
    public float currentMP;
    Room room;

    public HpAndManaHUD(Room room)
    {
        currentHP = 1; //max
        currentMP = 1; //max
        this.room = room;
    }

    public void RecieveHUD(string changeRate)
    {
        ChangeMP(changeRate);
        ChangeHP(changeRate);
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
    }

    public void ChangeMaxHP(string deltaMaxHP)
    {
        float valueMaxHP = float.Parse(deltaMaxHP);
        maxHP = valueMaxHP;
        ChangeHP(deltaMaxHP);
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
    }

    public void ChangeMaxMP(string deltaMaxMP)
    {
        float valueMaxMP = float.Parse(deltaMaxMP);
        maxMP = valueMaxMP;
        ChangeMP(deltaMaxMP);
    }

}
