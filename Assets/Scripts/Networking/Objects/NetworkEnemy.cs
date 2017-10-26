using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEnemy {

    public float posX;
    public float posY;
    public float hp;

    public bool fromEditor;
    public int id;

    public LevelManager levelManager;
    public Room room;

    public NetworkEnemy(int enemyId, float hp, Room room)
    {
        Debug.Log("New enemy " + enemyId + " with " + hp  + " hp");

        this.id = enemyId;
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
        room.SendMessageToAllPlayers("EnemyDie/" + id);
    }

    private bool IsDead()
    {
        return hp <= 0;
    }

}
