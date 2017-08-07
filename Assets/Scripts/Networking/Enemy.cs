using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {

    public Room room;
    public float hp;
    public float posX;
    public float posY;
    public int enemyId;
    public bool fromEditor;
    public LevelManager levelManager;

    public Enemy(int enemyId, Room room)
    {
        this.enemyId = enemyId;
        this.room = room;
    }

    public void ReduceHp(float hp)
    {
        this.hp -= hp;
        if (IsDead())
        {
            Die();
        }
    }

    private void Die()
    {
        room.SendMessageToAllPlayers("Die/" + enemyId);
    }

    private bool IsDead()
    {
        if (hp <= 0)
        {
            return true;
        }
        return false;
    }

}
