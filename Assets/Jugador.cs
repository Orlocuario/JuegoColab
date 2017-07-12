using UnityEngine;
using System.Collections;

public class Jugador
{
    public int connectionId;
    Room room;
    public bool connected;
    public int charId;

    public Jugador(int connectionId, int charId, Room room)
    {
        this.connectionId = connectionId;
        this.room = room;
        this.charId = charId;
        connected = true;
    }
}
