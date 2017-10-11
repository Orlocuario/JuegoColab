using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerPlayer : MonoBehaviour {

	public string name;
	public PlayerType type;
	public bool luring;
	public PlannerPoi playerAt;
	public List<PlannerItem> playerInventory;

	public PlannerPlayer(){
		this.playerInventory = new List<PlannerItem> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		Debug.Log ("Test Player inicio");
		List<string> def = new List<string> ();
		def.Add("(player-at " + name + " " + playerAt.name + ")");
		Debug.Log ("Test Player 2");
		if(luring && type == PlayerType.mage){
			Debug.Log ("Test Player 3");
			def.Add("(luring " + name + ")");
		}
		foreach (PlannerItem item in playerInventory) {
			Debug.Log ("Test Player 4");
			def.Add("(player-inventory " + name + " " + item.name + ")");
			Debug.Log ("Test Player 5");
		}
		Debug.Log ("Test Player 6");
		return def;
	}
}

public enum PlayerType{
	player = 0,
	mage = 1,
	warrior = 2,
	inventor = 3
}