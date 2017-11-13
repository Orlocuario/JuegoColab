using UnityEngine;
using System.Collections;
using System.IO;

public class RoomLogger
{
    int roomId;
    public RoomLogger(int id)
    {
        this.roomId = id;
    }

    StreamWriter GetWriter()
    {
        return new StreamWriter(File.Open(roomId + ".txt", FileMode.Append));
    }

    public void WriteNewPosition(int playerId, float positionX, float positionY, bool pressingJump, bool pressingLeft, bool pressingRight)
    {
        StreamWriter writer = GetWriter();
        string line = "";
        if (pressingJump)
        {
            line = "Player " + playerId + " jumped from (" + positionX + "," + positionY + ")"; 
        }
        else if(pressingLeft && !pressingRight)
        {
            line = "Player " + playerId + " is going left from (" + positionX + "," + positionY + ")";
        }
        else if (!pressingLeft && pressingRight)
        {
            line = "Player " + playerId + " is going right from (" + positionX + "," + positionY + ")";
        }
        else if(pressingRight && pressingLeft)
        {
            line = "Player " + playerId + " is pressing right AND left while standing in (" + positionX + "," + positionY + ")";
        }
        else
        {
            return;
        }
        writer.WriteLine(line);
        writer.Close();
    }

}
