using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpAndManaHUD {

    public float maxHP;
    public float maxMP;
    public float currentHP;
    public float currentMP;
    public float percentageHP;
    public float percentageMP;
    Room room;

    public HpAndManaHUD(Room room)
    {
        this.room = room;
        maxHP = 250;
        maxMP = 250;
        currentHP = maxHP;
        currentMP = maxMP;
        percentageHP = 1;
        percentageMP = 1;
    }

    public void RecieveHpAndMpHUD(string changeRate)
    {
        ChangeHP(changeRate);
        ChangeMP(changeRate);
    }

    public void ChangeHP(string deltaHP)
    {
        float valueDeltaHP = float.Parse(deltaHP);
        currentHP += valueDeltaHP;

        if (currentHP >= maxHP)
        {
            currentHP = maxHP;
        }
        else if (currentHP <= 0)
        {
            currentHP = 0;
            room.SendMessageToAllPlayers("PlayersAreDead");
        }

        percentageHP = currentHP / maxHP;
        room.SendMessageToAllPlayers("DisplayChangeHPToClient/" + percentageHP + ":" + currentHP + ":" + maxHP);
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

        if (currentMP >= maxMP)
        {
            currentMP = maxMP;
        }
        else if (currentMP <= 0)
        {
            currentMP = 0;
        }

        percentageMP = currentMP / maxMP;
        room.SendMessageToAllPlayers("DisplayChangeMPToClient/" + percentageMP + ":" + currentMP + ":" + maxMP);
    }

    public void ChangeMaxMP(string NewMaxMP)
    {
        float valueMaxMP = float.Parse(NewMaxMP);
        maxMP = valueMaxMP;
        ChangeMP(NewMaxMP);
    }

}
