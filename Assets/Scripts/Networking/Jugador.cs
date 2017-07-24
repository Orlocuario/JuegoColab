using UnityEngine;
using System.Collections;

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
}
