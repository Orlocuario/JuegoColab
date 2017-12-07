using System.Collections.Generic;

public class RoomDoors
{

    #region Attributes

    HashSet<string> openedDoors;

    #endregion

    #region Costructor

    public RoomDoors()
    {
        openedDoors = new HashSet<string>();
    }

    #endregion

    #region Common

    //Agrega una puerta
    public void AddDoor(string id)
    {
        openedDoors.Add(id);
    }

    //Elimina todas las entradas existentes. Puede ser util al empezar una nueva etapa.
    public void Reset()
    {
        openedDoors = new HashSet<string>();
    }

    //Retorna un string que se enviará a los clientes por cada puerta existente en la base de datos
    public List<string> GetDoorMessages()

    {
        List<string> messages = new List<string>();
        foreach (string door in openedDoors)
        {
            string message = "ActivateRuneSystem/" + door;
            messages.Add(message);
        }
        return messages;
    }

    #endregion

}
