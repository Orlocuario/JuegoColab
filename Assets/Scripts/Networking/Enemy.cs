using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {

    public float posX;
    public float posY;
    public float hp;

    public bool fromEditor;
    public int enemyId;

    public LevelManager levelManager;
    public Room room;

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
        return hp <= 0;
    }

}
