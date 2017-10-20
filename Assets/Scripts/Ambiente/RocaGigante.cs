using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocaGigante : MonoBehaviour
{

    private Vector3 lastPosition;
    private Vector3 myPosition;

    public PlannerObstacle obstacleObj = null;

    private void Start()
    {
        lastPosition = this.gameObject.GetComponent<Transform>().position;
    }

    private void Update()
    {

        if (Client.instance == null || Client.instance.GetWarrior() == null)
        {
            return;
        }

        myPosition = this.gameObject.GetComponent<Transform>().position;

        if (Client.instance.GetWarrior().localPlayer)
        {
            if (myPosition.ToString().Substring(0, 4) != lastPosition.ToString().Substring(0, 4))
            {
                lastPosition = myPosition;
                Client.instance.SendMessageToServer("ChangeObjectPosition/" +
                    this.gameObject.name + "/" +
                    myPosition.x.ToString() + "/" +
                    myPosition.y.ToString() + "/" +
                    myPosition.z.ToString());
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "TriggerRocaGigante")
        {
            if (obstacleObj != null)
            {
                obstacleObj.blocked = false;
                obstacleObj.open = true;
            }
            Client.instance.SendMessageToServer("InstantiateObject/Prefabs/Ambientales/SueloRoca");
            Client.instance.SendMessageToServer("DestroyObject/" + this.gameObject.name);
        }
    }
}
