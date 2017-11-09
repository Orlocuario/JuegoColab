using UnityEngine;
using System.Collections;

public class EndOfScene : MonoBehaviour
{

    private int playersWhoArrived;
    private int playersToArrive;
    LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        playersWhoArrived = 0;    
        playersToArrive = 3; // This could be managed dinamically
    }

    protected bool GameObjectIsPlayer(GameObject other)
    {
        if (other.tag == "Player")
        {
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController.localPlayer)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameObjectIsPlayer(other.gameObject))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
			levelManager.ActivateNPCLog ("Asegúrate de que lleguen todos tus amigos");
            Debug.Log(other.gameObject.name + " reached the end of the scene");

			if (++playersWhoArrived == playersToArrive) 
			{
				playerController.StopMoving ();
				Debug.Log ("All players reached the end of the scene");
				levelManager.GoToNextScene ();
			} 
        }
    }
	/*private void OnTriggerExit2D(Collider2D other)
	{
		if (GameObjectIsPlayer (other.gameObject)) {
			PlayerController playerController = other.gameObject.GetComponent<PlayerController> ();
			Debug.Log (other.gameObject.name + " left the end of the scene");
			--playersWhoArrived; 
		}
	*/
}
