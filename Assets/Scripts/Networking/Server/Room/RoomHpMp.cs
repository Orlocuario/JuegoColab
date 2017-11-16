﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomHpMp {

    public float maxHP;
    public float maxMP;
    public float maxExp;
    public float currentHP;
    public float currentMP;
    public float currentExp;
    public float percentageHP;
    public float percentageMP;
    public float percentageExp;
    private bool mpAtLimit = false;

    Room room;

    public RoomHpMp(Room room)
    {                         
        this.room = room;
        maxHP = 250;
        maxMP = 250;
        maxExp = 250;
        currentHP = maxHP;
        currentMP = maxMP;
        currentExp = 0;
        percentageHP = 1;
        percentageMP = 1;
        percentageExp = 0;
    }

    public void RecieveHpAndMpHUD(string changeRate)
    {
        ChangeHP(changeRate);
        ChangeMP(changeRate);
    }

    public void ChangeHP(string deltaHP)
    {
        float valueDeltaHP = float.Parse(deltaHP);
        if (valueDeltaHP == 0)
        {
            return;
        }
        currentHP += valueDeltaHP;

        if (currentHP >= maxHP)
        {
            currentHP = maxHP;
        }
        else if (currentHP <= 0)
        {
            currentHP = 0;
            room.SendMessageToAllPlayers("PlayersAreDead/" + Server.instance.sceneToLoad);
            currentHP = maxHP;
            currentMP = maxMP;
            room.SendMessageToAllPlayers("NewChatMessage/" + room.actualChat);
        }

        percentageHP = currentHP / maxHP;
        room.SendMessageToAllPlayers("DisplayChangeHPToClient/" + percentageHP);
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

        currentMP += valueDeltaMP;

        if (currentMP > maxMP)
        {
            currentMP = maxMP;
        }
        else if (currentMP <= 0)
        {
            currentMP = 0;
        }

        percentageMP = currentMP / maxMP;

        if(percentageMP == 1 || percentageMP == 0)
        {
            if (mpAtLimit)
            {
                return;
            }
            mpAtLimit = true;
        }
        else
        {
            mpAtLimit = false;
        }

        room.SendMessageToAllPlayers("DisplayChangeMPToClient/" + percentageMP.ToString());
    }

    public void ChangeMaxMP(string NewMaxMP)
    {
        float valueMaxMP = float.Parse(NewMaxMP);
        maxMP = valueMaxMP;
        ChangeMP(NewMaxMP);
    }

    public void ChangeExp(string deltaExp)
    {
        float valueDeltaExp = float.Parse(deltaExp);
        currentExp += valueDeltaExp;

        if (currentExp >= maxExp)
        {
            currentExp = 0;
            // levelUp
        }

        percentageExp = currentExp / maxExp;
        room.SendMessageToAllPlayers("DisplayChangeExpToClient/" + percentageExp);
    }

    public void ChangeMaxExp(string NewMaxExp)
    {
        float valueMaxExp = float.Parse(NewMaxExp);
        maxExp = valueMaxExp;
        ChangeMP(NewMaxExp);
    }
}