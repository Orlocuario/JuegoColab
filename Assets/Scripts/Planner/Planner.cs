using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner : MonoBehaviour {

	public bool control;
	public int level;
	public List<PlannerPlayer> playerList;
	public List<PlannerPoi> poiList;
	public List<PlannerObstacle> obstacleList;
	public List<PlannerItem> itemList;
	public List<PlannerSwitch> switchList;

	private string message;
	private List<string> objDef;
	private List<string> initDef;
	private List<string> EstadoInicial;
	//Plan completo
	private List<string> Plan;
	//Estado por accion
	private List<List<string>> EstadoPorAccion;
	//Estado negativo por accion
	private List<List<string>> EstadoNegativoPorAccion;

	public double coeficienteMaximo = 20;
	private double coeficienteActual;
	private int distanciaObjetiva = -1;
	private double timer;
	private int feedbackLevel = 0;
	private int etapaCumplida = 0;

	// Use this for initialization
	void Start () {
		message = "";
		objDef = new List<string> ();
		initDef = new List<string> ();
		EstadoInicial = new List<string> ();
		Plan = new List<string>();
		EstadoPorAccion = new List<List<string>> ();
		EstadoNegativoPorAccion = new List<List<string>> ();

		if (control) {
			Escaneo ();
			Replanificar ();
		}
	}

	void Update(){
		if (control) {
			timer += Time.deltaTime;
			coeficienteActual = distanciaObjetiva * timer / 60.0;
			if (coeficienteActual > coeficienteMaximo) {
				Debug.Log ("Feedback");
				timer = 0;
				if (etapaCumplida != Plan.Count) {
					this.RequestActivateNPCLog(GetFeedback (Plan [etapaCumplida]));
				}
			}
		}
	}

	//Metodo de replanificacion, toma el estado actual y lo envía al servidor.
	void Replanificar(){
		if (control) {
			Debug.Log ("Inicio replanificacion");
			//send message (estado actual) al server para planificar
			EstadoInicial = new List<string> (initDef);
			Client.instance.SendMessageToPlanner (this.message);
		}
	}
	//Metodo de recepcion del plan
	public void SetPlanFromServer(string message){
		if (control) {
			Debug.Log ("Recibido mensaje Server");
			Debug.Log (message);
			List<string> parameters = new List<string> (message.Split ('/'));
			parameters.RemoveAt (parameters.Count - 1);
			this.Plan = new List<string> (parameters);
			distanciaObjetiva = this.Plan.Count;
			Debug.Log ("distancia objetiva: " + distanciaObjetiva);
			GetEstadosDesdePlanEstandar ();
		}
	}
	//Metodo de calculo estado por accion
	void GetEstadosDesdePlanEstandar(){
		if (control) {
			List<string> estadoActual = new List<string> (EstadoInicial);
			this.EstadoPorAccion = new List<List<string>> ();
			this.EstadoPorAccion.Add (new List<string> (estadoActual));
			foreach (string action in this.Plan) {
				estadoActual = new List<string> (GetEstadoPorAccionEstandar (estadoActual, action));
				this.EstadoPorAccion.Add (new List<string> (estadoActual));
			}
		}
	}
	//Metodo de escaneo, revisa el estado actual y lo guarda en las variables correspondientes
	void Escaneo(){
		if (control) {
			objDef = new List<string> ();
			initDef = new List<string> ();
			message = level + "/";
			foreach (PlannerPlayer player in playerList) {
				objDef.Add (player.GetDefinitionObjects ());
			}
			foreach (PlannerPoi poi in poiList) {
				objDef.Add (poi.GetDefinitionObjects ());
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				objDef.Add (obstacle.GetDefinitionObjects ());
			}
			foreach (PlannerItem item in itemList) {
				objDef.Add (item.GetDefinitionObjects ());
			}
			foreach (PlannerSwitch switchItem in switchList) {
				objDef.Add (switchItem.GetDefinitionObjects ());
			}
			foreach (string item in objDef) {
				message += item + " ";
			}
			message += "/";

			foreach (PlannerPlayer player in playerList) {
				initDef.AddRange (player.GetDefinitionInit ());
			}
			foreach (PlannerPoi poi in poiList) {
				initDef.AddRange (poi.GetDefinitionInit ());
			}
			foreach (PlannerObstacle obstacle in obstacleList) {
				initDef.AddRange (obstacle.GetDefinitionInit ());
			}
			foreach (PlannerItem item in itemList) {
				initDef.AddRange (item.GetDefinitionInit ());
			}
			foreach (PlannerSwitch switchItem in switchList) {
				initDef.AddRange (switchItem.GetDefinitionInit ());
			}

			foreach (string item in initDef) {
				message += item;
			}
		}
	}
	//Metodo monitor (con cada cambio de accion se llama y decide si sse debe replanificar o no
	public void Monitor(){
		if (control) {
			bool cumple = false;
			Escaneo ();
			Debug.Log ("comienzo de monitoreo");
			for (int i = 0; i <= this.Plan.Count; i++) {
				List<string> estadoActual = new List<string> (EstadoPorAccion [i]);
				if (estadoActual.Count == initDef.Count && initDef.All (estadoActual.Contains)) {
					Debug.Log ("cumple estado:" + i);
					etapaCumplida = i;
					distanciaObjetiva = this.Plan.Count - i;
					Debug.Log ("distancia objetiva: " + distanciaObjetiva);
					cumple = true;
					break;
				}
			}
			if (!cumple) {
				Debug.Log ("no cumple estados");
				this.Replanificar ();
				etapaCumplida = 0;
			}
		}
	}
	//Mapeo estado por accion, guarda el estado para la accion i del plan
	List<string> GetEstadoPorAccionEstandar(List<string> estadoActual, string accion){
		if (control) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				foreach (PlannerObstacle enemy in obstacleList) {
					if (enemy.type == ObstacleType.enemy && estadoActual.Contains ("(enemy-at " + enemy.name + " " + parametros [1] + ")") && !estadoActual.Contains ("(enemy-at " + enemy.name + " " + parametros [2] + ")") && estadoActual.Contains ("(luring " + parametros [0] + ")")) {
						estadoActual.Add ("(blocked " + enemy.name + ")");
						estadoActual.Remove ("(luring " + parametros [0] + ")");
					}
				}
				break;
			case "move-jump":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "move-distract":
				estadoActual.Remove ("(blocked " + parametros [3] + ")");
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(luring " + parametros [0] + ")");
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				break;
			case "lever-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "lever-off":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "machine-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable && estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(rollable-locked " + obstacle.name + ")");
					}
				}
				break;
			case "item-pick":
				estadoActual.Remove ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				break;
			case "item-drop":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				break;
			case "rune-use":
				foreach (PlannerPoi poi in poiList) {
					if (estadoActual.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && estadoActual.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && estadoActual.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						estadoActual.Remove ("(blocked " + parametros [3] + ")");
					}
				}
				break;
			case "gear-use":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(machine-loaded " + parametros [3] + ")");
				break;
			case "step-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "push-boulder":
				estadoActual.Remove ("(blocked " + parametros [1] + ")");
				break;
			}
			return estadoActual;
		}
		return null;
	}

	//Mapeo estado por accion, guarda el estado para la accion i del plan
	List<string> GetEstadoPorAccionRegresion(List<string> estadoActual, string accion){
		if (control) {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				foreach (PlannerObstacle enemy in obstacleList) {
					if (enemy.type == ObstacleType.enemy && estadoActual.Contains ("(enemy-at " + enemy.name + " " + parametros [1] + ")") && !estadoActual.Contains ("(enemy-at " + enemy.name + " " + parametros [2] + ")") && !estadoActual.Contains ("(luring " + parametros [0] + ")")) {
						estadoActual.Remove ("(blocked " + enemy.name + ")");
					}
				}
				estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.enemy && estadoActual.Contains ("(enemy-at " + obstacle.name + " " + parametros [1] + ")") && !estadoActual.Contains ("(enemy-at " + obstacle.name + " " + parametros [2] + ")") && !estadoActual.Contains ("(luring " + parametros [0] + ")")) {
						if (!estadoActual.Contains ("(enemy-at " + obstacle.name + " " + parametros [1] + ")")) {
							estadoActual.Add ("(enemy-at " + obstacle.name + " " + parametros [1] + ")");
						}
						if (!estadoActual.Contains ("(luring " + parametros [0] + ")")) {
							estadoActual.Add ("(luring " + parametros [0] + ")");
						}
					}
				}
				break;
			case "move-jump":
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				break;
			case "move-distract":
				estadoActual.Remove ("(luring " + parametros [0] + ")");
				estadoActual.Remove ("(player-at " + parametros [0] + " " + parametros [2] + ")");
				if (!estadoActual.Contains ("(player-at " + parametros [0] + " " + parametros [1] + ")")) {
					estadoActual.Add ("(player-at " + parametros [0] + " " + parametros [1] + ")");
				}
				if (!estadoActual.Contains ("(enemy-at " + parametros [3] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(enemy-at " + parametros [3] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-to " + parametros [1] + " " + parametros [2] + ")")) {
					estadoActual.Add ("(route-to " + parametros [1] + " " + parametros [2] + ")");
				}
				if (!estadoActual.Contains ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")")) {
					estadoActual.Add ("(route-block " + parametros [1] + " " + parametros [2] + " " + parametros [3] + ")");
				}
				if (!estadoActual.Contains ("(blocked " + parametros [3] + ")")) {
					estadoActual.Add ("(blocked " + parametros [3] + ")");
				}
				break;
			case "lever-on":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "lever-off":
				estadoActual.Remove ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Add ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "machine-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (obstacle.type == ObstacleType.rollable && estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(rollable-locked " + obstacle.name + ")");
					}
				}
				break;
			case "item-pick":
				estadoActual.Remove ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				estadoActual.Add ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				break;
			case "item-drop":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(item-at " + parametros [1] + " " + parametros [2] + ")");
				break;
			case "rune-use":
				foreach (PlannerPoi poi in poiList) {
					if (estadoActual.Contains ("(route-to " + parametros [2] + " " + poi.name + ")") && estadoActual.Contains ("(route-block " + parametros [2] + " " + poi.name + " " + parametros [3] + ")") && estadoActual.Contains ("(door-route " + parametros [2] + " " + poi.name + " " + parametros [3] + ")")) {
						estadoActual.Remove ("(blocked " + parametros [3] + ")");
					}
				}
				break;
			case "gear-use":
				estadoActual.Remove ("(player-inventory " + parametros [0] + " " + parametros [1] + ")");
				estadoActual.Add ("(machine-loaded " + parametros [3] + ")");
				break;
			case "step-on":
				estadoActual.Add ("(switch-on " + parametros [1] + ")");
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [1] + " " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "triple-switch":
				foreach (PlannerObstacle obstacle in obstacleList) {
					if (estadoActual.Contains ("(linked-switch " + parametros [3] + " " + obstacle.name + ")") && estadoActual.Contains ("(blocked " + obstacle.name + ")")) {
						estadoActual.Remove ("(blocked " + obstacle.name + ")");
					}
				}
				break;
			case "push-boulder":
				estadoActual.Remove ("(blocked " + parametros [1] + ")");
				break;
			}
			return estadoActual;
		}
		return null;
	}

	string GetFeedback(string accion){
		string feedback = "";
		if (feedbackLevel == 0) {
			feedback = "Aun no han llegado, ¿Está todo bien?";
		} else {
			List<string> parametros = new List<string> (accion.Split (new char[]{ '(', ')', ' ' }));
			parametros.RemoveAt (0);
			string nombre = parametros [0];
			parametros.RemoveAt (0);
			switch (nombre) {
			case "move":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + parametros [0] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "¿Creo que no han pasado por aqui aún?";
					break;
				case 3:
					feedback = "¿Probaron ir hasta " + parametros [2] + "?";
					break;
				default:
					feedback = parametros [0] + ", debe ir desde " + parametros [1] + " hasta " + parametros [2];
					break;
				}
				break;
			case "move-jump":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + parametros [0] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "No olviden sus habilidades...";
					break;
				case 3:
					feedback = parametros [0] + ", no olvides tu habilidad especial";
					break;
				case 4:
					feedback = "¿Probaron ir a " + parametros [2] + "?";
					break;
				default:
					feedback = parametros [0] + " debería usar su salto alto para llegar a " + parametros [2];
					break;
				}
				break;
			case "move-distract":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + parametros [0] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "No olviden sus habilidades...";
					break;
				case 3:
					feedback = "¿Probaron ir a " + parametros [2] + "?";
					break;
				case 4:
					feedback = parametros [0] + ", no olvides tu habilidad especial";
					break;
				default:
					feedback = parametros [0] + " debería usar su campo magico para pasar " + parametros [3];
					break;
				}
				break;
			case "lever-on":
				switch (feedbackLevel) {
				case 1:
					feedback = "Esperaba ver a " + parametros [0] + " por aquí, ¿sabes que le pasa?";
					break;
				case 2:
					feedback = "No olviden sus habilidades...";
					break;
				case 3:
					feedback = "¿Probaron ir a " + parametros [2] + "?";
					break;
				case 4:
					feedback = parametros [0] + ", no olvides tu habilidad especial";
					break;
				default:
					feedback = parametros [0] + " debería usar su campo magico para pasar " + parametros [3];
					break;
				}
				break;
			case "lever-off":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "machine-on":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "item-pick":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "item-drop":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "rune-use":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "gear-use":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "step-on":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "triple-switch":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			case "push-boulder":
				switch (feedbackLevel) {
				case 1:
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				}
				break;
			}
		}
		feedbackLevel++;
		return feedback;
	}

	void RequestActivateNPCLog(string feedbackMessage)
	{
		Client.instance.SendMessageToServer("ActivateNPCLog/" + feedbackMessage);
	}

}