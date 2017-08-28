using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActions : MonoBehaviour {

    int groupId;

    public SwitchActions(GroupOfSwitchs group)
    {
        this.groupId = group.groupId;
        foreach(Switch switchi in group.GetSwitchs())
        {
            switchi.SetJobDone();
        }
    }

	public void DoSomething()
	{
		switch (groupId) {
		case 3789:
			//Animator animadorRoca = GameObject.FindGameObjectWithTag ("rocaql").GetComponent<Animator> ();
			//animadorRoca.SetBool ("rocasecae", true);
			break;

		case 0:
			GameObject platescalerain = (GameObject)Instantiate (Resources.Load ("Prefabs/EnginPlath1"));
			platescalerain.GetComponent<Transform> ().position = new Vector2 (12.9f, -1.4f);
			GameObject feedbackswitchin = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitchin.GetComponent<Transform> ().position = new Vector2 (8.224f, -0.976f);
			break;

		case 1:
                GameObject sujetaRoca = GameObject.FindGameObjectWithTag("SujetaRoca");
                sujetaRoca.GetComponent<Animator>().SetBool("isSwitch", true);
               
			break;
		case 2:
			GameObject platescalera = (GameObject)Instantiate (Resources.Load ("Prefabs/MagePlath"));
			platescalera.GetComponent<Transform> ().position = new Vector2 (43f, -16f);
			GameObject feedbackswitch = (GameObject)Instantiate (Resources.Load ("Prefabs/FeedbackParticles/FBMageButt"));
			feedbackswitch.GetComponent<Transform> ().position = new Vector2 (41.4f, -16.3f);
			break;
		default:
			break;

		//GameObject tevoyamataroe = GameObject.FindGameObjectWithTag ("plataforma1");
		//GameObject.Destroy (tevoyamataroe);
		//GameObject fireball = (GameObject)Instantiate (Resources.Load ("Prefabs/Attacks/BolaM1"));
		//fireball.GetComponent<Transform> ().position = new Vector2 (3, 4);*/
			}
		}
	}