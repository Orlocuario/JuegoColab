using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannerObstacle : MonoBehaviour {

	public string name;
	public ObstacleType type;
	public List<PlannerPoi> enemyAt;
	public bool blocked;
	public List<PlannerItem> doorRune;
	public bool rollableLocked;

	public PlannerObstacle(){
		this.enemyAt = new List<PlannerPoi> ();
		this.doorRune = new List<PlannerItem> ();
	}

	public string GetDefinitionObjects(){
		string message = "";
		message += name + " - " + type.ToString ();
		return message;
	}

	public List<string> GetDefinitionInit(){
		List<string> def = new List<string> ();
		if (type == ObstacleType.enemy) {
			foreach (PlannerPoi item in enemyAt) {
				def.Add ("(enemy-at " + name + " " + item.name + ")");
			}
		}
		if (blocked) {
			def.Add("(blocked " + name + ")");
		}
		if(type == ObstacleType.door){
			foreach (PlannerItem item in doorRune) {
				if(item.type == ItemType.rune){
					def.Add("(door-rune " + name + " " + item.name + ")");
				}
			}
		}
		if(rollableLocked && type == ObstacleType.rollable){
			def.Add("(rollable-locked " + name + ")");
		}
		return def;
	}
}

public enum ObstacleType{
	obstacle = 0,
	rollable = 1,
	door = 2,
	jump = 3,
	enemy = 4
}