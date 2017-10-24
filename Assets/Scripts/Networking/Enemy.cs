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

    public Enemy(int enemyId, float hp, Room room)
    {
        Debug.Log("New enemy " + enemyId + " with " + hp  + " hp");

        this.enemyId = enemyId;
        this.room = room;
        this.hp = hp;
    }

    public void ReduceHp(float damage)
    {
        this.hp -= damage;
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
