using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class RoomLogger 
{
    int roomId;
    public RoomLogger(int id)
    {
        this.roomId = id;
    }

    StreamWriter GetWriter()
    {
        return new StreamWriter(File.Open("Log_room_" +roomId + ".txt", FileMode.Append));
    }

    public void WriteAttack(int playerId)
    {
        StreamWriter writer = GetWriter();
        writer.WriteLine("Player " + playerId + " attacked\n");
        writer.Close();
    }

    public void WritePower(int playerId, bool powerState)
    {
        StreamWriter writer = GetWriter();
        if (powerState)
        {
            writer.WriteLine("Player " + playerId + " used his power\n");
        }
        else
        {
            writer.WriteLine("Player " + playerId + "stopped using his power\n");
        }
        writer.Close();
    }

    //Modificar si se cambia el sistema de inventario
    public void WriteInventory(int charId, string message)
    {
        char[] separator = new char[1];
        separator[0] = '/';
        string[] msg = message.Split(separator);
        int index = Int32.Parse(msg[2]);

        StreamWriter writer = GetWriter();

        if (msg[1] == "Add")
        {
            writer.WriteLine("Player " + charId + " picked stored " + msg[3] + " in the slot " + index +"\n");
        }
        else
        {
            writer.WriteLine("Player " + charId + " tossed item in slot " + index + "\n");
        }
        writer.Close();

    }
    public void WriteNewPosition(int playerId, float positionX, float positionY, bool pressingJump, bool pressingLeft, bool pressingRight)
    {
        StreamWriter writer = GetWriter();
        string line = "";
        if (pressingJump)
        {
            line = "Player " + playerId + " jumped from (" + positionX + "," + positionY + ")\n"; 
        }
        else if(pressingLeft && !pressingRight)
        {
            line = "Player " + playerId + " is going left from (" + positionX + "," + positionY + ")\n";
        }
        else if (!pressingLeft && pressingRight)
        {
            line = "Player " + playerId + " is going right from (" + positionX + "," + positionY + ")\n";
        }
        else if(pressingRight && pressingLeft)
        {
            line = "Player " + playerId + " is pressing right AND left while standing in (" + positionX + "," + positionY + ")\n";
        }
        else
        {
            writer.Close();
            return;
        }
        writer.WriteLine(line);
        writer.Close();
    }

}
