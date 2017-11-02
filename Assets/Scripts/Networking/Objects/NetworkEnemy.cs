using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEnemy {

    public float hp;

    public bool fromEditor;
    public int instanceId;
    public int id;
    
    public LevelManager levelManager;
    public Room room;

    public NetworkEnemy(int instanceId, int enemyId, float hp, Room room)
    {
        this.instanceId = instanceId;
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
