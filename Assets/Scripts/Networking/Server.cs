using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System;
using System.Threading;

public class Server : MonoBehaviour {

    public int maxConnections;
    int socketId;
    int channelId;
    int bigChannelId;
    int timesScene1IsLoaded;
    public List<Room> rooms;
    public ServerMessageHandler messageHandler;
    public static Server instance;
    public int maxJugadores;
    public string sceneToLoad;
    public string NPCsLastMessage;

    //Planner Thread
	Thread planner;
	//Cola de mensajes a procesar
	private List<string> messageStack = new List<string>();
	//Cola de conections id del output del planner
	private List<int> connectionIdStack = new List<int>();
	//Cola de mensajes procesados por el planner
	private List<string> outputStack = new List<string>();
	//Ruta y nombre del template del archivo problema
	public string templateFileName;
	//Ruta y nombre del archivo problema nuevo
	public string problemFileName;
	//Ruta y nombre del archivo batch para planificar
	public string batchFileName;
	//Ruta y nombre del archivo output de la planificacion
	public string outputFileName;
	//Donde se debe escribir para cada template
	public List<int> startLinePerLevel;


    // Use this for initialization
    void Start()
    {

        NPCsLastMessage = "";
        maxJugadores = 1;
        instance = this;
        timesScene1IsLoaded = 0;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Unreliable);
        bigChannelId = config.AddChannel(QosType.ReliableFragmented);
        HostTopology topology = new HostTopology(config, maxConnections);
        socketId = NetworkTransport.AddHost(topology, NetConsts.port);
        rooms = new List<Room>();
        messageHandler = new ServerMessageHandler(this);
        planner = new Thread(new ThreadStart(this.Plan));
	    planner.Start ();
        this.sceneToLoad = "Escena2";
    }

    // Update is called once per frame
    void LateUpdate ()
    {
		if (connectionIdStack.Count > 0)
		{
			int connectionId = connectionIdStack [0];
			string output = outputStack [0];
			SendPlannerInfoToClient (connectionId, output);
			connectionIdStack.RemoveAt (0);
			outputStack.RemoveAt (0);
		}
        int recSocketId;
        int recConnectionId; // Reconoce la ID del jugador
        int recChannelId;
        byte[] recBuffer = new byte[NetConsts.bufferSize];
        int dataSize;
        byte error;
        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetConsts.bufferSize, out dataSize, out error);
        NetworkError Error = (NetworkError)error;
        if (Error == NetworkError.MessageToLong)
        {
            //Trata de capturar el mensaje denuevo, pero asumiendo buffer más grande.
            recBuffer = new byte[NetConsts.bigBufferSize];
            recNetworkEvent = NetworkTransport.Receive(out recSocketId, out recConnectionId, out recChannelId, recBuffer, NetConsts.bigBufferSize, out dataSize, out error);
        }
        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                AddConnection(recConnectionId);
                UnityEngine.Debug.Log("Client " + recConnectionId + " connected");
                break;
            case NetworkEventType.DataEvent:
                Stream stream = new MemoryStream(recBuffer);
                BinaryFormatter formatter = new BinaryFormatter();
                string message = formatter.Deserialize(stream) as string;
                string hora = HoraMinuto();

                UnityEngine.Debug.Log(hora + " - From: " + recConnectionId + " Handling: " + message);

                if (recChannelId == channelId)
                {
                    //Mensaje corto normal
                    messageHandler.HandleMessage(message, recConnectionId);

                }
                if (recChannelId == bigChannelId)
                {
                    //Mensaje largo. Planner
                    SendMessagToPlanner(message, recConnectionId);
                }

                break;
            case NetworkEventType.DisconnectEvent:
                DeleteConnection(recConnectionId);
                UnityEngine.Debug.Log("Client " + recConnectionId + " disconnected");
                break;
        }
    }

    private string HoraMinuto()
    {
        string hora = DateTime.Now.Hour.ToString();
        string minutos = DateTime.Now.Minute.ToString();

        if (minutos.Length == 1)
        {
            minutos = "0" + minutos;
        }

        string tiempo = " (" + hora + ":" + minutos + ")";
        return tiempo;
    }

    public void SendMessageToClient(int clientId, string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[NetConsts.bufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, clientId, channelId, buffer, NetConsts.bufferSize, out error);
    }

    public void SendPlannerInfoToClient(int clientId, string message)
    {
        byte error;
        //int bytes = System.Text.ASCIIEncoding.ASCII.GetByteCount(message);
        byte[] buffer = new byte[NetConsts.bigBufferSize];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, message);
        NetworkTransport.Send(socketId, clientId, bigChannelId, buffer, NetConsts.bigBufferSize, out error);
    }

    public void SendMessageToClient(Jugador player, string message)
    {
        SendMessageToClient(player.connectionId, message);
    }

    private void AddConnection(int connectionId)
    {
        //Jugador existía y se reconecta.
        string recAddress;
        int port;
        UnityEngine.Networking.Types.NetworkID recNetId;
        UnityEngine.Networking.Types.NodeID recNodeId;
        byte recError;
        NetworkTransport.GetConnectionInfo(socketId, connectionId, out recAddress, out port, out recNetId, out recNodeId, out recError);
        Jugador player = GetPlayer(recAddress);
        if (player != null)
        {
            player.connected = true;
            player.connectionId = connectionId;
            SendMessageToClient(connectionId, "ChangeScene/" + sceneToLoad);
            timesScene1IsLoaded += 1;
            messageHandler.SendAllData(connectionId, player.room);
            UnityEngine.Debug.Log("Client "  + connectionId + " reconnected");
            return;
        }

        //Jugador no existía y se crea uno nuevo.
        Room room = SearchRoom();
        if(room == null)
        {
            room = new Room(rooms.Count, this, messageHandler, maxJugadores);
            rooms.Add(room);
        }

        room.AddPlayer(connectionId, recAddress);
    }



    private void DeleteConnection(int connectionId)
    {
        Jugador player = GetPlayer(connectionId);
        if (player != null)
        {
            player.connected = false;
            int charId = player.charId;
            string role;
            if (charId == 0)
            {
                role = "Mage: Has Disconnected";
            }
            else if (charId == 1)
            {
                role = "Warrior: Has Disconnected";
            }
            else
            {
                role = "Engineer: Has Disconnected";
            }
            player.room.SendMessageToAllPlayers("NewChatMessage/" + role);
            if(player.controlOverEnemies == true)
            {
                player.room.ChangeControlEnemies();
            }
        }
    }

    public Jugador GetPlayer(int connectionId)
    {
        foreach (Room room in rooms)
        {
            Jugador player = room.FindPlayerInRoom(connectionId);
            if (player != null)
            {
                return room.FindPlayerInRoom(connectionId);
            }
        }
        return null;
    }

    public Jugador GetPlayer(string address)
    {
        foreach (Room room in rooms)
        {
            Jugador player = room.FindPlayerInRoom(address);
            if (player != null)
            {
                return room.FindPlayerInRoom(address);
            }
        }
        return null;
    }
    //Retorna la sala con la mayor cantidad de jugadores que no esté llena.
    private Room SearchRoom()
    {
        Room selectedRoom = null;
        int selectedMaxPlayers = 0;
        foreach(Room room in rooms)
        {
            if (!room.IsFull())
            {
                if(selectedMaxPlayers <= room.numJugadores)
                {
                    selectedRoom = room;
                    selectedMaxPlayers = room.numJugadores;
                }
            }
        }
        return selectedRoom;
    }

    private void SendMessagToPlanner(string message, int connectionId)
    {
		messageStack.Add (connectionId + "/" + message);
    }

	private void Plan()
	{
		while (true)
		{
			if (messageStack != null && messageStack.Count > 0) {
				UnityEngine.Debug.Log ("stack enter count:" + messageStack.Count);
				string message = messageStack [0];
				messageStack.RemoveAt (0);
				List<string> parameters = new List<string> (message.Split ('/'));
				int connectionId = int.Parse (parameters [0]);
				parameters.RemoveAt (0);
				int level = int.Parse (parameters [0]);
				parameters.RemoveAt (0);
				string def = parameters [0];
				string init = parameters [1];
				string goal = parameters [2];
				List<string> data = new List<string> ();
				data.Add (def);
				data.Add (")");
				data.Add ("(:init");
				data.Add (init);
				data.Add (")");
				data.Add ("(:goal (and");
				data.Add (goal);
				string tempFileName = templateFileName + level + ".txt";
				string probFileName = problemFileName + level + ".pddl";
				string batFileName = batchFileName + level + ".bat";
				string outFileName = outputFileName + level + ".txt";
				List<string> lines = new List<string> (System.IO.File.ReadAllLines (tempFileName));
				lines.InsertRange (startLinePerLevel [level - 1], data);
				using (StreamWriter writer = new StreamWriter (probFileName, false)) {
					foreach (string line in lines) {
						writer.WriteLine (line);
					}
				}
				Process batchProcess = new Process ();
				batchProcess.StartInfo.UseShellExecute = false;
				batchProcess.StartInfo.RedirectStandardOutput = true;
				batchProcess.StartInfo.CreateNoWindow = true;
				batchProcess.StartInfo.FileName = batFileName;
				string output = null;
				try {
					batchProcess.Start ();
					UnityEngine.Debug.Log ("batch star");
					output = batchProcess.StandardOutput.ReadToEnd();
					UnityEngine.Debug.Log (output);
					batchProcess.WaitForExit ();
					UnityEngine.Debug.Log ("batch ended");
					batchProcess.Close();
					UnityEngine.Debug.Log ("batch close");
					List<string> linesOutput = new List<string> (System.IO.File.ReadAllLines (outFileName));
					if(linesOutput.Count>0){
						output = linesOutput[0];
						linesOutput.RemoveAt(0);
						foreach(string line in linesOutput)
						{
							output += "/" + line;
						}

					}
				} catch (FileNotFoundException e) {
					output = "ERROR";
					UnityEngine.Debug.Log (output);
				}
				catch (Exception e){
					output = e.ToString ();
					UnityEngine.Debug.Log (output);
				}
				//Send output
				connectionIdStack.Add (connectionId);
				outputStack.Add (output);
			}
		}
	}
}
