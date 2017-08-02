using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Jugador
{
    public int connectionId;
    public Room room;
    public bool connected;
    public int charId;
    public float positionX;
    public float positionY;
    public bool isGrounded;
    public float speed;
    public int direction;
    public bool pressingJump;
    public bool pressingRight;
    public bool pressingLeft;
    public bool attacking;
    public string[] inventory = new string[8];

    public Jugador(int connectionId, int charId, Room room)
    {
        this.connectionId = connectionId;
        this.room = room;
        this.charId = charId;
        connected = true;
        positionX = 0;
        positionY = 0;
        isGrounded = false;
        speed = 0;
        direction = 1;
        pressingJump = false;
        pressingRight = false;
        pressingLeft = false;
        attacking = false;
    }

    public void InventoryUpdate(string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] arreglo = message.Split(separator);
        int index = Int32.Parse(arreglo[2]);

        if (arreglo[1] == "Add")
        {
            AddItemToInventory(index, arreglo[3]);
        }
        else
        {
            RemoveItemFromInventory(index);
        }
    }

    private void AddItemToInventory(int index, string spriteName)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i == index)
            {
                inventory[i] = spriteName;
                return;
            }
        }
    }

    private void RemoveItemFromInventory(int index)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (i == index)
            {
                inventory[i] = null;
                return;
            }
        }
    }
}
